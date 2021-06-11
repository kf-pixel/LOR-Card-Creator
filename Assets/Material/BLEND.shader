Shader "Custom/Blend" {
    Properties {
        _MainTex ("Texture to blend", 2D) = "black" {}
        _Color ("Main Color", Color) = (1,1,1,1)
    }
    SubShader {
        Tags { "Queue" = "AlphaTest" "RenderType"="TransparentCutout" "IgnoreProjector"="True" }
        Pass {
            Blend OneMinusDstColor One
            ZWrite Off
            AlphaToMask On
            ColorMask RGB


            SetTexture [_MainTex] { combine texture }
        }
    }
}