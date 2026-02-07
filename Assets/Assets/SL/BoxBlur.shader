// BoxBlur.shader
Shader "Hidden/BoxBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _KernelSize ("Kernel Size", Float) = 3.0
    }
    SubShader
    {
        Pass
        { // Pass 0: Horizontal Blur
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag_h
            #include "UnityCG.cginc"
            struct v2f { float4 pos : SV_POSITION; half2 uv : TEXCOORD0; };
            v2f vert (appdata_img v) { v2f o; o.pos = UnityObjectToClipPos(v.vertex); o.uv = v.texcoord; return o; }
            sampler2D _MainTex; float4 _MainTex_TexelSize; float _KernelSize;
            half4 frag_h (v2f i) : SV_Target
            {
                half4 sum = half4(0,0,0,0);
                float blurPixels = _KernelSize * 0.5;
                for(float x = -blurPixels; x <= blurPixels; x += 1.0)
                    sum += tex2D(_MainTex, i.uv + float2(x * _MainTex_TexelSize.x, 0.0));
                return sum / (_KernelSize + 1.0);
            }
            ENDCG
        }
        Pass
        { // Pass 1: Vertical Blur
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag_v
            #include "UnityCG.cginc"
            struct v2f { float4 pos : SV_POSITION; half2 uv : TEXCOORD0; };
            v2f vert (appdata_img v) { v2f o; o.pos = UnityObjectToClipPos(v.vertex); o.uv = v.texcoord; return o; }
            sampler2D _MainTex; float4 _MainTex_TexelSize; float _KernelSize;
            half4 frag_v (v2f i) : SV_Target
            {
                half4 sum = half4(0,0,0,0);
                float blurPixels = _KernelSize * 0.5;
                for(float y = -blurPixels; y <= blurPixels; y += 1.0)
                    sum += tex2D(_MainTex, i.uv + float2(0.0, y * _MainTex_TexelSize.y));
                return sum / (_KernelSize + 1.0);
            }
            ENDCG
        }
    }
}