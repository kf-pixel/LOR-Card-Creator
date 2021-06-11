// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/BLUR" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _BumpAmt  ("Distortion", Range (0,128)) = 10
        _Tint ("Tint Color (RGB)", 2D) = "white" {}
        _BumpMap ("Normalmap", 2D) = "bump" {}
        _Size ("Size", Range(0, 20)) = 1

         // see Stencil in UI/Default
        [HideInInspector]_StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector]_Stencil ("Stencil ID", Float) = 0
        [HideInInspector]_StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector]_StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector]_StencilReadMask ("Stencil Read Mask", Float) = 255
        [HideInInspector]_ColorMask ("Color Mask", Float) = 15
        [HideInInspector]_UseUIAlphaClip ("Use Alpha Clip", Float) = 0

    }
 
    Category {

        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Opaque" }
 
 
        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        SubShader {
     
            GrabPass {                    
                Tags { "LightMode" = "Always" }
            }
            Pass {
                Tags { "LightMode" = "Always" }
             
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma fragmentoption ARB_precision_hint_fastest
                #include "UnityCG.cginc"
             
                struct appdata_t {
                    float4 vertex : POSITION;
                    float2 texcoord: TEXCOORD0;
                };
             
                struct v2f {
                    float4 vertex : POSITION;
                    float4 uvgrab : TEXCOORD0;
                     float2 alphaUV : TEXCOORD1;

                };

                sampler2D _MainTex;
                 float4 _MainTex_ST;
             
                v2f vert (appdata_t v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    #if UNITY_UV_STARTS_AT_TOP
                    float scale = -1.0;
                    #else
                    float scale = 1.0;
                    #endif
                    o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
                    o.uvgrab.zw = o.vertex.zw;

                                         o.alphaUV = TRANSFORM_TEX(v.texcoord, _MainTex);


                    return o;
                }
             
                sampler2D _GrabTexture;
                float4 _GrabTexture_TexelSize;
                float _Size;
             
                half4 frag( v2f i ) : COLOR {

                    half4 sum = half4(0,0,0,0);
                    #define GRABPIXEL(weight,kernelx) tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(float4(i.uvgrab.x + _GrabTexture_TexelSize.x * kernelx*_Size * tex2D(_MainTex, i.alphaUV).a, i.uvgrab.y, i.uvgrab.z, i.uvgrab.w))) * weight
                    

                     sum += GRABPIXEL(0.05, -4.0);
                     sum += GRABPIXEL(0.09, -3.0);
                     sum += GRABPIXEL(0.12, -2.0);
                     sum += GRABPIXEL(0.15, -1.0);
                     sum += GRABPIXEL(0.18,  0.0);
                     sum += GRABPIXEL(0.15, +1.0);
                     sum += GRABPIXEL(0.12, +2.0);
                     sum += GRABPIXEL(0.09, +3.0);
                     sum += GRABPIXEL(0.05, +4.0);

                    /*
                    sum += GRABPIXEL(0.015, -9.0);
 		            sum += GRABPIXEL(0.020, -8.0);
                    sum += GRABPIXEL(0.025, -7.0);
                    sum += GRABPIXEL(0.035, -6.0);
                    sum += GRABPIXEL(0.045, -5.0);
                    sum += GRABPIXEL(0.055, -4.0);
                    sum += GRABPIXEL(0.065, -3.0);
                    sum += GRABPIXEL(0.08, -2.0);
                    sum += GRABPIXEL(0.10, -1.0);
                    sum += GRABPIXEL(0.12, 0.0);
                    sum += GRABPIXEL(0.10, +1.0);
                    sum += GRABPIXEL(0.08, +2.0);
                    sum += GRABPIXEL(0.065, +3.0);
                    sum += GRABPIXEL(0.055, +4.0);
                    sum += GRABPIXEL(0.045, +5.0);
                    sum += GRABPIXEL(0.035, +6.0);
                    sum += GRABPIXEL(0.025, +7.0);
                    sum += GRABPIXEL(0.020, +8.0);
                    sum += GRABPIXEL(0.015, +9.0);
                    */
   
                 
                    return sum;
                }
                ENDCG
            }

            GrabPass {                        
                Tags { "LightMode" = "Always" "Queue" = "Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
            }
            Pass {
                Tags { "LightMode" = "Always" }
                         Blend SrcAlpha OneMinusSrcAlpha
             
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma fragmentoption ARB_precision_hint_fastest
                #include "UnityCG.cginc"
             
                struct appdata_t {
                    float4 vertex : POSITION;
                    float2 texcoord: TEXCOORD0;
                };
             
                struct v2f {
                    float4 vertex : POSITION;
                    float4 uvgrab : TEXCOORD0;
                    float2 alphaUV : TEXCOORD1;
                };

                 sampler2D _MainTex;
                 float4 _MainTex_ST;
             
                v2f vert (appdata_t v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    #if UNITY_UV_STARTS_AT_TOP
                    float scale = -1.0;
                    #else
                    float scale = 1.0;
                    #endif
                    o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
                    o.uvgrab.zw = o.vertex.zw;

                     o.alphaUV = TRANSFORM_TEX(v.texcoord, _MainTex);


                    return o;
                }
             
                sampler2D _GrabTexture;
                float4 _GrabTexture_TexelSize;
                float _Size;
             
                half4 frag( v2f i ) : COLOR {
                 
                    half4 sum = half4(0,0,0,0);
                    #define GRABPIXEL(weight,kernely) tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(float4(i.uvgrab.x, i.uvgrab.y + _GrabTexture_TexelSize.y * kernely*_Size * tex2D(_MainTex, i.alphaUV).a, i.uvgrab.z, i.uvgrab.w))) * weight
                 
                    sum += GRABPIXEL(0.015, -9.0);
 		            sum += GRABPIXEL(0.020, -8.0);
                    sum += GRABPIXEL(0.025, -7.0);
                    sum += GRABPIXEL(0.035, -6.0);
                    sum += GRABPIXEL(0.045, -5.0);
                    sum += GRABPIXEL(0.055, -4.0);
                    sum += GRABPIXEL(0.065, -3.0);
                    sum += GRABPIXEL(0.08, -2.0);
                    sum += GRABPIXEL(0.10, -1.0);
                    sum += GRABPIXEL(0.12, 0.0);
                    sum += GRABPIXEL(0.10, +1.0);
                    sum += GRABPIXEL(0.08, +2.0);
                    sum += GRABPIXEL(0.065, +3.0);
                    sum += GRABPIXEL(0.055, +4.0);
                    sum += GRABPIXEL(0.045, +5.0);
                    sum += GRABPIXEL(0.035, +6.0);
                    sum += GRABPIXEL(0.025, +7.0);
                    sum += GRABPIXEL(0.020, +8.0);
                    sum += GRABPIXEL(0.015, +9.0);
                 
                    return sum;
                }
                ENDCG
            }
         
            GrabPass {                        
                Tags { "LightMode" = "Always" }
            }
            Pass {
                Tags { "LightMode" = "Always" "Queue" = "Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
                Blend SrcAlpha OneMinusSrcAlpha
             
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma fragmentoption ARB_precision_hint_fastest
                #include "UnityCG.cginc"
             
                struct appdata_t {
                    float4 vertex : POSITION;
                    float2 texcoord: TEXCOORD0;
                };
             
                struct v2f {
                    float4 vertex : POSITION;
                    float4 uvgrab : TEXCOORD0;
                    float2 uvbump : TEXCOORD1;
                    float2 uvmain : TEXCOORD2;
                };
             
                float _BumpAmt;
                float4 _BumpMap_ST;
                float4 _Tint_ST;
             
                v2f vert (appdata_t v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    #if UNITY_UV_STARTS_AT_TOP
                    float scale = -1.0;
                    #else
                    float scale = 1.0;
                    #endif
                    o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
                    o.uvgrab.zw = o.vertex.zw;
                    o.uvbump = TRANSFORM_TEX( v.texcoord, _BumpMap );
                    o.uvmain = TRANSFORM_TEX( v.texcoord, _Tint );
                    return o;
                }
             
                fixed4 _Color;
                sampler2D _GrabTexture;
                float4 _GrabTexture_TexelSize;
                sampler2D _BumpMap;
                sampler2D _Tint;
             
                half4 frag( v2f i ) : COLOR {
              
                    half2 bump = UnpackNormal(tex2D( _BumpMap, i.uvbump )).rg;
                    float2 offset = bump * _BumpAmt * _GrabTexture_TexelSize.xy;
                    i.uvgrab.xy = offset * i.uvgrab.z + i.uvgrab.xy;
                 
                    half4 col = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(i.uvgrab));
                    half4 tint = tex2D( _Tint, i.uvmain ) * _Color;
                 
                    return col * tint;
                }
                ENDCG
            }

            
        }
    }
}