// Shader created with Shader Forge Beta 0.34 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.34;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:0,uamb:True,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.01411765,fgcg:0.03573605,fgcb:0.07058823,fgca:1,fgde:0.005,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:32719,y:32712|emission-39-OUT,custl-4-OUT;n:type:ShaderForge.SFN_LightAttenuation,id:2,x:33861,y:32676;n:type:ShaderForge.SFN_LightColor,id:3,x:33861,y:32544;n:type:ShaderForge.SFN_Multiply,id:4,x:33321,y:32712|A-3-RGB,B-2-OUT,C-18-OUT;n:type:ShaderForge.SFN_LightVector,id:5,x:34107,y:32779;n:type:ShaderForge.SFN_NormalVector,id:6,x:34107,y:32925,pt:True;n:type:ShaderForge.SFN_Dot,id:7,x:33926,y:32895,dt:1|A-5-OUT,B-6-OUT;n:type:ShaderForge.SFN_Code,id:16,x:33589,y:32818,code:aQBmACgAYQAgAD4AIAAwAC4AOABmACkACgAJAHIAZQB0AHUAcgBuACAAMQAuADAAZgA7AAoAZQBsAHMAZQAgAGkAZgAoAGEAIAA+ACAAMAAuADUAZgApAAoACQAJAHIAZQB0AHUAcgBuACAAMAAuADYAZgA7AAoAZQBsAHMAZQAgAGkAZgAoAGEAIAA+ACAAMAAuADIAZgApAAoACQByAGUAdAB1AHIAbgAgADAALgA0AGYAOwAKAGUAbABzAGUACgAJAHIAZQB0AHUAcgBuACAAMAAuADEAZgA7AAoACQAKAAkA,output:0,fname:LightClamp,width:247,height:132,input:0,input_1_label:a|A-7-OUT;n:type:ShaderForge.SFN_Tex2d,id:17,x:33663,y:33043,ptlb:MainTex,ptin:_MainTex,tex:f0ea96ec319f8384b92092b0cc26a39e,ntxv:0,isnm:False|UVIN-47-UVOUT;n:type:ShaderForge.SFN_Multiply,id:18,x:33394,y:32987|A-16-OUT,B-17-RGB;n:type:ShaderForge.SFN_AmbientLight,id:37,x:33277,y:32436;n:type:ShaderForge.SFN_Multiply,id:39,x:33100,y:32573|A-37-RGB,B-17-RGB;n:type:ShaderForge.SFN_Panner,id:47,x:33903,y:33064,spu:0.6,spv:0.7|DIST-218-OUT;n:type:ShaderForge.SFN_Time,id:211,x:34172,y:33172;n:type:ShaderForge.SFN_Multiply,id:218,x:33992,y:33244|A-211-TSL,B-220-OUT;n:type:ShaderForge.SFN_ValueProperty,id:220,x:34172,y:33343,ptlb:Pan Speed,ptin:_PanSpeed,glob:False,v1:1;proporder:17-220;pass:END;sub:END;*/

Shader "Dissertation/ToonPanning" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _PanSpeed ("Pan Speed", Float ) = 1
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float4 _TimeEditor;
            float LightClamp( float a ){
            if(a > 0.8f)
            	return 1.0f;
            else if(a > 0.5f)
            		return 0.6f;
            else if(a > 0.2f)
            	return 0.4f;
            else
            	return 0.1f;
            	
            	
            }
            
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _PanSpeed;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Normals:
                float3 normalDirection =  i.normalDir;
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
////// Emissive:
                float4 node_211 = _Time + _TimeEditor;
                float2 node_47 = (i.uv0.rg+(node_211.r*_PanSpeed)*float2(0.6,0.7));
                float4 node_17 = tex2D(_MainTex,TRANSFORM_TEX(node_47, _MainTex));
                float3 emissive = (UNITY_LIGHTMODEL_AMBIENT.rgb*node_17.rgb);
                float3 finalColor = emissive + (_LightColor0.rgb*attenuation*(LightClamp( max(0,dot(lightDirection,normalDirection)) )*node_17.rgb));
/// Final Color:
                return fixed4(finalColor,1);
            }
            ENDCG
        }
        Pass {
            Name "ForwardAdd"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            Fog { Color (0,0,0,0) }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float4 _TimeEditor;
            float LightClamp( float a ){
            if(a > 0.8f)
            	return 1.0f;
            else if(a > 0.5f)
            		return 0.6f;
            else if(a > 0.2f)
            	return 0.4f;
            else
            	return 0.1f;
            	
            	
            }
            
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _PanSpeed;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Normals:
                float3 normalDirection =  i.normalDir;
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float4 node_211 = _Time + _TimeEditor;
                float2 node_47 = (i.uv0.rg+(node_211.r*_PanSpeed)*float2(0.6,0.7));
                float4 node_17 = tex2D(_MainTex,TRANSFORM_TEX(node_47, _MainTex));
                float3 finalColor = (_LightColor0.rgb*attenuation*(LightClamp( max(0,dot(lightDirection,normalDirection)) )*node_17.rgb));
/// Final Color:
                return fixed4(finalColor * 1,0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
