Shader "Custom/TerrainClouds"
{
    Properties
    {
        // Cloud shadow opacity
        _CloudAlpha ("Cloud Opacity", Range(0, 0.5)) = 0.2
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        
        // 1. Use the standard terrain pass
        UsePass "Nature/Terrain/Diffuse/BASE"
        
        // 2. Add our ultra-simple cloud overlay
        Pass
        {
            Name "CLOUDS"
            Blend Zero SrcAlpha  // Darken what's underneath
            ZWrite Off  // Don't write to depth buffer
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            float _CloudAlpha;
            
            float4 vert(float4 vertex : POSITION) : SV_POSITION
            {
                return UnityObjectToClipPos(vertex);
            }
            
            fixed4 frag() : SV_Target
            {
                // Just a constant gray shadow
                return fixed4(0.7, 0.7, 0.7, _CloudAlpha);
            }
            ENDCG
        }
    }
    
    Fallback "Nature/Terrain/Diffuse"
}