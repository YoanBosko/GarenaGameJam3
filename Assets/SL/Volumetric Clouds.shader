Shader "Skybox/Multi-Layer Clouds Complete"
{
    Properties
    {
        // ======================
        // SMALL CUMULUS CLOUDS
        // ======================
        [Header(Small Cumulus Clouds)]
        _SmallScale ("Small Scale", Range(0.1, 10)) = 2.0
        _SmallSpeed ("Small Speed", Range(0, 0.5)) = 0.05
        _SmallCoverage ("Small Coverage", Range(0, 1)) = 0.4
        _SmallDensity ("Small Density", Range(0, 2)) = 1.0
        _SmallSharpness ("Small Sharpness", Range(1, 30)) = 8.0
        _SmallAltitude ("Small Altitude", Range(0.1, 0.9)) = 0.3
        _SmallThickness ("Small Thickness", Range(0.05, 0.5)) = 0.2
        _SmallColor ("Small Color", Color) = (1, 1, 1, 1)
        
        // ======================
        // LARGE CUMULUS CLOUDS
        // ======================
        [Header(Large Cumulus Clouds)]
        _LargeScale ("Large Scale", Range(0.1, 10)) = 1.5
        _LargeSpeed ("Large Speed", Range(0, 0.5)) = 0.03
        _LargeCoverage ("Large Coverage", Range(0, 1)) = 0.3
        _LargeDensity ("Large Density", Range(0, 2)) = 0.8
        _LargeSharpness ("Large Sharpness", Range(1, 30)) = 6.0
        _LargeAltitude ("Large Altitude", Range(0.1, 0.9)) = 0.5
        _LargeThickness ("Large Thickness", Range(0.05, 0.5)) = 0.25
        _LargeColor ("Large Color", Color) = (1, 1, 1, 1)
        
        // ======================
        // CIRRUS CLOUDS
        // ======================
        [Header(Cirrus Clouds)]
        _CirrusScale ("Cirrus Scale", Range(0.1, 10)) = 3.0
        _CirrusSpeed ("Cirrus Speed", Range(0, 0.5)) = 0.02
        _CirrusCoverage ("Cirrus Coverage", Range(0, 1)) = 0.2
        _CirrusDensity ("Cirrus Density", Range(0, 2)) = 0.4
        _CirrusSharpness ("Cirrus Sharpness", Range(1, 30)) = 3.0
        _CirrusAltitude ("Cirrus Altitude", Range(0.1, 0.9)) = 0.8
        _CirrusThickness ("Cirrus Thickness", Range(0.05, 0.5)) = 0.1
        _CirrusColor ("Cirrus Color", Color) = (0.9, 0.95, 1.0, 1)
        
        // ======================
        // GLOBAL SETTINGS
        // ======================
        [Header(Global Settings)]
        _SunDirection ("Sun Direction", Vector) = (0.3, 0.6, 0.7, 0)
        _SunIntensity ("Sun Intensity", Range(0, 10)) = 2.0
        _SunColor ("Sun Color", Color) = (1, 0.9, 0.7, 1)
        
        _SkyTop ("Sky Top", Color) = (0.37, 0.52, 0.73, 1)
        _SkyBottom ("Sky Bottom", Color) = (0.89, 0.96, 1, 1)
        _HorizonColor ("Horizon Color", Color) = (0.89, 0.89, 0.89, 1)
        
        _TimeScale ("Time Scale", Float) = 1.0
        
        // Layer toggles
        [Toggle]_EnableSmall ("Enable Small Cumulus", Float) = 1
        [Toggle]_EnableLarge ("Enable Large Cumulus", Float) = 1
        [Toggle]_EnableCirrus ("Enable Cirrus", Float) = 1
    }
    
    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Cull Off
        ZWrite Off

        // ======================
        // PASS 1: MAIN SKYBOX RENDERING
        // ======================
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 texcoord : TEXCOORD0;
            };

            // Small Cumulus properties
            float _SmallScale, _SmallSpeed, _SmallCoverage, _SmallDensity, _SmallSharpness;
            float _SmallAltitude, _SmallThickness;
            float4 _SmallColor;

            // Large Cumulus properties
            float _LargeScale, _LargeSpeed, _LargeCoverage, _LargeDensity, _LargeSharpness;
            float _LargeAltitude, _LargeThickness;
            float4 _LargeColor;

            // Cirrus properties
            float _CirrusScale, _CirrusSpeed, _CirrusCoverage, _CirrusDensity, _CirrusSharpness;
            float _CirrusAltitude, _CirrusThickness;
            float4 _CirrusColor;

            // Global properties
            float4 _SunDirection;
            float _SunIntensity;
            float4 _SunColor;
            float4 _SkyTop, _SkyBottom, _HorizonColor;
            float _TimeScale;
            float _EnableSmall, _EnableLarge, _EnableCirrus;

            // ======================
            // NOISE FUNCTIONS
            // ======================

            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453);
            }

            float noise(float2 uv)
            {
                float2 i = floor(uv);
                float2 f = frac(uv);

                float2 u = f * f * (3.0 - 2.0 * f);

                float a = hash(i);
                float b = hash(i + float2(1.0, 0.0));
                float c = hash(i + float2(0.0, 1.0));
                float d = hash(i + float2(1.0, 1.0));

                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
            }

            float fbm(float2 uv)
            {
                float value = 0.0;
                float amplitude = 0.5;
                float frequency = 1.0;

                for (int i = 0; i < 4; i++)
                {
                    value += amplitude * noise(uv * frequency);
                    amplitude *= 0.5;
                    frequency *= 2.0;
                }

                return value;
            }

            // ======================
            // SPHERICAL CLOUD MAPPING
            // ======================
            float2 directionToCloudUV(float3 dir, float altitude, float time, float speed, float scale)
            {
                // Convert direction to spherical coordinates
                float phi = atan2(dir.z, dir.x);   // Horizontal angle
                float theta = asin(dir.y);         // Vertical angle (use asin for better distribution)

                // Adjust for altitude - this is the key fix!
                // We need to sample clouds based on the spherical direction, not absolute height
                float2 uv;
                uv.x = phi * 0.159154943;  // 1/(2π) - horizontal
                uv.y = theta * 0.318309886 + altitude;  // 1/π - vertical, offset by altitude

                // Apply scale and movement
                uv *= scale;
                uv.x += time * speed;

                return uv;
            }

            // ======================
            // CLOUD LAYER FUNCTIONS
            // ======================

            // Add this function and modify getCloudLayer to use it:
            float3 getTriPlanarWeights(float3 normal)
            {
                float3 weights = abs(normal);
                weights = pow(weights, 4.0);  // Sharpens the blend
                weights = weights / (weights.x + weights.y + weights.z);
                return weights;
            }

            float getCloudLayer(float3 dir, float time, float scale, float speed, 
                                    float coverage, float density, float sharpness,
                                    float altitude, float thickness)
            {
                // Get blending weights for three projections
                float3 weights = getTriPlanarWeights(dir);

                // Generate UVs for three projections
                float2 uvX = (dir.zy * scale) + float2(time * speed, 0);
                float2 uvY = (dir.xz * scale) + float2(time * speed * 0.7, 0);
                float2 uvZ = (dir.xy * scale) + float2(time * speed * 0.3, 0);

                // Sample each projection
                float cloudsX = fbm(uvX) * weights.x;
                float cloudsY = fbm(uvY) * weights.y;
                float cloudsZ = fbm(uvZ) * weights.z;

                // Combine
                float clouds = cloudsX + cloudsY + cloudsZ;

                // Height-based fade
                float heightFactor = saturate((dir.y + 1.0) * 0.5);  // Map from [-1,1] to [0,1]
                float altitudeFade = 1.0 - saturate(abs(heightFactor - altitude) / thickness);
                clouds *= altitudeFade;

                // Horizon fade
                float horizonFade = exp(-(1.0 - abs(dir.y)) * 3.0);
                clouds *= horizonFade;

                // Apply final parameters
                clouds = saturate((clouds - (1.0 - coverage)) * sharpness);
                clouds *= density;

                return clouds;
            }

            // Small Cumulus
            float getSmallCumulus(float3 dir, float time)
            {
                if (_EnableSmall < 0.5) return 0.0;
                return getCloudLayer(dir, time, _SmallScale, _SmallSpeed, 
                                   _SmallCoverage, _SmallDensity, _SmallSharpness,
                                   _SmallAltitude, _SmallThickness);
            }

            // Large Cumulus
            float getLargeCumulus(float3 dir, float time)
            {
                if (_EnableLarge < 0.5) return 0.0;
                return getCloudLayer(dir, time, _LargeScale, _LargeSpeed * 0.8, 
                                   _LargeCoverage, _LargeDensity, _LargeSharpness,
                                   _LargeAltitude, _LargeThickness);
            }

            // Cirrus (special treatment for wispy appearance)
            float getCirrus(float3 dir, float time)
            {
                if (_EnableCirrus < 0.5) return 0.0;

                float2 uv = directionToCloudUV(dir, _CirrusAltitude, time, _CirrusSpeed, _CirrusScale);

                // High frequency for wispy appearance
                float clouds = fbm(uv * 5.0) * 0.5 + fbm(uv * 10.0) * 0.25;

                // Very thin and wispy
                clouds = pow(clouds, 3.0);

                // Height falloff
                float verticalPos = uv.y / _CirrusScale;
                float heightFalloff = 1.0 - saturate(abs(verticalPos - _CirrusAltitude) / (_CirrusThickness * 0.5));
                clouds *= heightFalloff;

                // Apply parameters
                clouds = saturate((clouds - (1.0 - _CirrusCoverage)) * _CirrusSharpness);
                clouds *= _CirrusDensity * 0.3;

                // Cirrus should be visible even near horizon
                float horizon = 1.0 - abs(dir.y);
                clouds *= saturate(1.0 - horizon * 0.5);

                return clouds;
            }

            // ======================
            // LIGHTING & COLOR
            // ======================

            float3 calculateCloudColor(float density, float4 baseColor, float3 dir)
            {
                float3 sunDir = normalize(_SunDirection.xyz);

                // Sun lighting based on direction
                float sunDot = dot(dir, sunDir);
                float lighting = saturate(sunDot * 0.5 + 0.5);

                // Height-based color (lighter when looking up)
                float heightFactor = saturate(dir.y * 0.5 + 0.5);
                float3 color = baseColor.rgb;

                // Sun highlights (brighter when looking toward sun)
                float highlight = pow(saturate(sunDot + 0.3), 3.0) * _SunIntensity;
                color = lerp(color, color * _SunColor.rgb, highlight * 0.5);

                // Apply density
                color *= saturate(density * 2.0);

                return color;
            }

            // Improved sky gradient that works with spherical coordinates
            float3 getSkyColor(float3 dir)
            {
                // Basic vertical gradient based on Y coordinate
                float t = smoothstep(-0.3, 0.8, dir.y);
                float3 skyColor = lerp(_SkyBottom.rgb, _SkyTop.rgb, t);

                // Horizon glow (stronger at horizon)
                float horizon = 1.0 - abs(dir.y);
                float glow = pow(horizon, 2.0) * 0.7;
                skyColor = lerp(skyColor, _HorizonColor.rgb, glow);

                // Sun glow
                float3 sunDir = normalize(_SunDirection.xyz);
                float sunGlow = pow(saturate(dot(dir, sunDir)), 10.0);
                skyColor += _SunColor.rgb * sunGlow * _SunIntensity * 0.2;

                return skyColor;
            }

            // ======================
            // VERTEX & FRAGMENT
            // ======================

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 viewDir = normalize(i.texcoord);
                float time = _Time.y * _TimeScale;

                // Sky background
                float3 skyColor = getSkyColor(viewDir);

                // Initialize cloud accumulators
                float3 totalCloudColor = float3(0, 0, 0);
                float totalCloudDensity = 0.0;

                // Get cloud densities for this view direction
                float smallDensity = getSmallCumulus(viewDir, time);
                float largeDensity = getLargeCumulus(viewDir, time);
                float cirrusDensity = getCirrus(viewDir, time);

                // Calculate colors for each layer
                if (smallDensity > 0.01)
                {
                    float3 smallColor = calculateCloudColor(smallDensity, _SmallColor, viewDir);
                    totalCloudColor = lerp(totalCloudColor, smallColor, saturate(smallDensity * 3.0));
                    totalCloudDensity = max(totalCloudDensity, smallDensity);
                }

                if (largeDensity > 0.01)
                {
                    float3 largeColor = calculateCloudColor(largeDensity, _LargeColor, viewDir);
                    // Large clouds should partially obscure small clouds
                    float blend = saturate(largeDensity * 2.5);
                    totalCloudColor = lerp(totalCloudColor, largeColor, blend);
                    totalCloudDensity = max(totalCloudDensity, largeDensity);
                }

                if (cirrusDensity > 0.01)
                {
                    // Cirrus are additive (thin and wispy)
                    float3 cirrusColor = _CirrusColor.rgb * cirrusDensity * 0.5;
                    totalCloudColor += cirrusColor;
                    totalCloudDensity += cirrusDensity * 0.2;
                }

                // FINAL COMPOSITION
                // Blend clouds with sky based on total density
                float cloudBlend = saturate(totalCloudDensity * 2.0);
                float3 finalColor = lerp(skyColor, totalCloudColor, cloudBlend);

                // Additional horizon fade for realism
                float horizon = 1.0 - abs(viewDir.y);
                float horizonFade = smoothstep(0.0, 0.4, horizon);
                finalColor = lerp(finalColor, skyColor, horizonFade * 0.5);

                // Ensure valid colors
                finalColor = max(finalColor, 0.0);

                return fixed4(finalColor, 1.0);
            }
            ENDCG
        }
    }
    // End of SubShader

    Fallback "Skybox/Cubemap"
}