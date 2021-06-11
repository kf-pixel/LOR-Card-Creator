// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/3COLOURGRADIENT"
{
    Properties
    {
         [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
         _ColorLeft ("Left Color", Color) = (1,1,1,1)
         _ColorMid ("Mid Color", Color) = (1,1,1,1)
         _ColorRight ("Right Color", Color) = (1,1,1,1)
         _Middle ("Center Point", Range(0.001, 0.999)) = 1

        // from hsv shader
		_hue ("Hue",Range(0.0,1.0)) = 1.0
		_saturation ("Saturation",Range(0.0,1.0)) = 1.0
		_value ("Value (Brightness)",Range(0.0,1.0)) = 1.0

        // see Stencil in UI/Default
        [HideInInspector]_StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector]_Stencil ("Stencil ID", Float) = 0
        [HideInInspector]_StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector]_StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector]_StencilReadMask ("Stencil Read Mask", Float) = 255
        [HideInInspector]_ColorMask ("Color Mask", Float) = 15
        [HideInInspector]_UseUIAlphaClip ("Use Alpha Clip", Float) = 0		
     }
 
     SubShader
     {
         Tags {"Queue"="Background"  "IgnoreProjector"="True"}

         LOD 100
         ZWrite On

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
 
         Pass
         {
            CGPROGRAM
            #pragma vertex vert  
            #pragma fragment frag
            #include "UnityCG.cginc"
    
            fixed4 _ColorLeft;
            fixed4 _ColorMid;
            fixed4 _ColorRight;
            float  _Middle;
            float _hue, _saturation, _value;

            struct v2f 
            {
                float4 pos : SV_POSITION; 
                float4 texcoord : TEXCOORD0;
            };
 
            v2f vert (appdata_full v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos (v.vertex);
                o.texcoord = v.texcoord;
                return o;
            }

            float3 RGBToHSV(float3 c)
            {
                float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
                float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
                float d = q.x - min( q.w, q.y );
                float e = 1.0e-10;
                return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
            }

            float3 HSVToRGB( float3 c )
            {
                float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
                float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
                return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
            }
 
            fixed4 frag (v2f i) : COLOR
            {
                fixed4 c =
                lerp(_ColorLeft, _ColorMid, i.texcoord.x / _Middle) * step(i.texcoord.x, _Middle);
                c +=
                lerp(_ColorMid, _ColorRight, (i.texcoord.x - _Middle) / (1 - _Middle)) * step(_Middle, i.texcoord.x);

                float3 source = RGBToHSV(c.rgb);
                source *= float3(_hue, _saturation, _value);
                c.rgb = HSVToRGB(source);

                c.a = 1;
                return c;
            }
         ENDCG
         }
     }
}
