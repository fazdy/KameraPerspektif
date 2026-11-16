Shader "Grafkom/World Surface"
{
    Properties
    {
        _Color ("Warna", Color) = (1, 1, 1, 1)
        _MainTex ("Tekstur", 2D) = "white" {}
        _EmissionColor ("Emisi", Color) = (0, 0, 0, 0)
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert fullforwardshadows addshadow
        #pragma target 3.0

        sampler2D _MainTex;
        fixed4 _Color;
        fixed4 _EmissionColor;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf(Input input, inout SurfaceOutput output)
        {
            output.Albedo = (tex2D(_MainTex, input.uv_MainTex) * _Color).rgb;
            output.Emission = _EmissionColor.rgb;
            output.Alpha = 1;
        }
        ENDCG
    }

    Fallback "Diffuse"
}
