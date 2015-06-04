// Shader created with Shader Forge Beta 0.34 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.34;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:0,uamb:True,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.01411765,fgcg:0.03573605,fgcb:0.07058823,fgca:1,fgde:0.005,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:32338,y:32781|diff-18-OUT,normal-31-OUT,emission-147-OUT,custl-161-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:2,x:33812,y:32509,ptlb:Control,ptin:_Control,glob:False,tex:f0ea96ec319f8384b92092b0cc26a39e;n:type:ShaderForge.SFN_Tex2dAsset,id:3,x:33593,y:32701,ptlb:Splat0,ptin:_Splat0,glob:False,tex:f0ea96ec319f8384b92092b0cc26a39e;n:type:ShaderForge.SFN_Tex2dAsset,id:4,x:33590,y:32895,ptlb:Splat1,ptin:_Splat1,glob:False,tex:0e8ee9651b2ea854e98f106265591e15;n:type:ShaderForge.SFN_Tex2dAsset,id:5,x:33590,y:33092,ptlb:Splat2,ptin:_Splat2,glob:False,tex:f0ea96ec319f8384b92092b0cc26a39e;n:type:ShaderForge.SFN_Tex2dAsset,id:6,x:33590,y:33274,ptlb:Splat3,ptin:_Splat3,glob:False,tex:f0ea96ec319f8384b92092b0cc26a39e;n:type:ShaderForge.SFN_Tex2d,id:7,x:33593,y:32526,tex:f0ea96ec319f8384b92092b0cc26a39e,ntxv:0,isnm:False|TEX-2-TEX;n:type:ShaderForge.SFN_Tex2d,id:8,x:33434,y:32701,tex:f0ea96ec319f8384b92092b0cc26a39e,ntxv:0,isnm:False|TEX-3-TEX;n:type:ShaderForge.SFN_Tex2d,id:9,x:33434,y:32895,tex:0e8ee9651b2ea854e98f106265591e15,ntxv:0,isnm:False|TEX-4-TEX;n:type:ShaderForge.SFN_Tex2d,id:10,x:33434,y:33092,tex:f0ea96ec319f8384b92092b0cc26a39e,ntxv:0,isnm:False|TEX-5-TEX;n:type:ShaderForge.SFN_Tex2d,id:11,x:33434,y:33274,tex:f0ea96ec319f8384b92092b0cc26a39e,ntxv:0,isnm:False|TEX-6-TEX;n:type:ShaderForge.SFN_Append,id:12,x:33434,y:32526|A-7-RGB,B-7-A;n:type:ShaderForge.SFN_Tex2dAsset,id:13,x:33199,y:32526,ptlb:MainTex,ptin:_MainTex,glob:False;n:type:ShaderForge.SFN_Tex2d,id:14,x:33031,y:32526,ntxv:0,isnm:False|TEX-13-TEX;n:type:ShaderForge.SFN_ChannelBlend,id:15,x:33194,y:32910,chbt:0|M-12-OUT,R-8-RGB,G-9-RGB,B-10-RGB,A-11-RGB;n:type:ShaderForge.SFN_Color,id:16,x:33031,y:32685,ptlb:Color,ptin:_Color,glob:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Lerp,id:17,x:32798,y:32685|A-14-RGB,B-16-RGB,T-20-OUT;n:type:ShaderForge.SFN_Lerp,id:18,x:32606,y:32644|A-15-OUT,B-17-OUT,T-20-OUT;n:type:ShaderForge.SFN_Vector1,id:20,x:32798,y:32865,v1:0;n:type:ShaderForge.SFN_Tex2dAsset,id:27,x:33590,y:33483,ptlb:Normal0,ptin:_Normal0,glob:False,tex:f0ea96ec319f8384b92092b0cc26a39e;n:type:ShaderForge.SFN_Tex2dAsset,id:28,x:33590,y:33683,ptlb:Normal1,ptin:_Normal1,glob:False;n:type:ShaderForge.SFN_Tex2dAsset,id:29,x:33590,y:33875,ptlb:Normal2,ptin:_Normal2,glob:False;n:type:ShaderForge.SFN_Tex2dAsset,id:30,x:33590,y:34073,ptlb:Normal3,ptin:_Normal3,glob:False;n:type:ShaderForge.SFN_ChannelBlend,id:31,x:33122,y:33693,chbt:0|M-12-OUT,R-33-RGB,G-34-RGB,B-35-RGB,A-36-RGB;n:type:ShaderForge.SFN_Tex2d,id:33,x:33395,y:33483,tex:f0ea96ec319f8384b92092b0cc26a39e,ntxv:0,isnm:False|TEX-27-TEX;n:type:ShaderForge.SFN_Tex2d,id:34,x:33388,y:33683,ntxv:0,isnm:False|TEX-28-TEX;n:type:ShaderForge.SFN_Tex2d,id:35,x:33382,y:33875,ntxv:0,isnm:False|TEX-29-TEX;n:type:ShaderForge.SFN_Tex2d,id:36,x:33382,y:34073,ntxv:0,isnm:False|TEX-30-TEX;n:type:ShaderForge.SFN_AmbientLight,id:146,x:32951,y:32947;n:type:ShaderForge.SFN_Multiply,id:147,x:32716,y:32987|A-146-RGB,B-18-OUT;n:type:ShaderForge.SFN_LightColor,id:154,x:33119,y:33116;n:type:ShaderForge.SFN_LightAttenuation,id:155,x:33119,y:33256;n:type:ShaderForge.SFN_LightVector,id:156,x:33149,y:33415;n:type:ShaderForge.SFN_NormalVector,id:157,x:33149,y:33547,pt:False;n:type:ShaderForge.SFN_Dot,id:158,x:32935,y:33470,dt:1|A-156-OUT,B-157-OUT;n:type:ShaderForge.SFN_Code,id:159,x:32615,y:33470,code:aQBmACgAYQAgAD4AIAAwAC4AOABmACkACgByAGUAdAB1AHIAbgAgADEALgAwAGYAOwAKAGUAbABzAGUAIABpAGYAKABhACAAPgAgADAALgA1AGYAKQAKAHIAZQB0AHUAcgBuACAAMAAuADYAZgA7AAoAZQBsAHMAZQAgAGkAZgAoAGEAIAA+ACAAMAAuADIAZgApAAoAcgBlAHQAdQByAG4AIAAwAC4ANABmADsACgBlAGwAcwBlAAoAcgBlAHQAdQByAG4AIAAwAC4AMQBmADsA,output:0,fname:LightClamp,width:247,height:132,input:0,input_1_label:a|A-158-OUT;n:type:ShaderForge.SFN_Multiply,id:160,x:32404,y:33520|A-159-OUT,B-18-OUT;n:type:ShaderForge.SFN_Multiply,id:161,x:32839,y:33219|A-160-OUT,B-154-RGB,C-155-OUT;proporder:2-3-4-5-6-13-16-27-28-29-30;pass:END;sub:END;*/

Shader "Dissertation/ToonTerrainAddPass" {
    Properties {
        _Control ("Control", 2D) = "white" {}
        _Splat0 ("Splat0", 2D) = "white" {}
        _Splat1 ("Splat1", 2D) = "white" {}
        _Splat2 ("Splat2", 2D) = "white" {}
        _Splat3 ("Splat3", 2D) = "white" {}
        _MainTex ("MainTex", 2D) = "white" {}
        _Color ("Color", Color) = (0.5,0.5,0.5,1)
        _Normal0 ("Normal0", 2D) = "white" {}
        _Normal1 ("Normal1", 2D) = "white" {}
        _Normal2 ("Normal2", 2D) = "white" {}
        _Normal3 ("Normal3", 2D) = "white" {}
    }
    SubShader {
        Tags {
			"SplatCount" = "4"
			"Queue"="Geometry-99"
            "RenderType"="Transparent"
			"IgnoreProjector"="True"
        }
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One One
            ZWrite Off
            
            Fog {Mode Off}
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
            uniform sampler2D _Control; uniform float4 _Control_ST;
            uniform sampler2D _Splat0; uniform float4 _Splat0_ST;
            uniform sampler2D _Splat1; uniform float4 _Splat1_ST;
            uniform sampler2D _Splat2; uniform float4 _Splat2_ST;
            uniform sampler2D _Splat3; uniform float4 _Splat3_ST;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _Color;
            uniform sampler2D _Normal0; uniform float4 _Normal0_ST;
            uniform sampler2D _Normal1; uniform float4 _Normal1_ST;
            uniform sampler2D _Normal2; uniform float4 _Normal2_ST;
            uniform sampler2D _Normal3; uniform float4 _Normal3_ST;
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
            
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 binormalDir : TEXCOORD4;
                LIGHTING_COORDS(5,6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
/////// Normals:
                float2 node_188 = i.uv0;
                float4 node_7 = tex2D(_Control,TRANSFORM_TEX(node_188.rg, _Control));
                float4 node_12 = float4(node_7.rgb,node_7.a);
                float3 normalLocal = (node_12.r*tex2D(_Normal0,TRANSFORM_TEX(node_188.rg, _Normal0)).rgb + node_12.g*tex2D(_Normal1,TRANSFORM_TEX(node_188.rg, _Normal1)).rgb + node_12.b*tex2D(_Normal2,TRANSFORM_TEX(node_188.rg, _Normal2)).rgb + node_12.a*tex2D(_Normal3,TRANSFORM_TEX(node_188.rg, _Normal3)).rgb);
                float3 normalDirection =  normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
////// Emissive:
                float node_20 = 0.0;
                float3 node_18 = lerp((node_12.r*tex2D(_Splat0,TRANSFORM_TEX(node_188.rg, _Splat0)).rgb + node_12.g*tex2D(_Splat1,TRANSFORM_TEX(node_188.rg, _Splat1)).rgb + node_12.b*tex2D(_Splat2,TRANSFORM_TEX(node_188.rg, _Splat2)).rgb + node_12.a*tex2D(_Splat3,TRANSFORM_TEX(node_188.rg, _Splat3)).rgb),lerp(tex2D(_MainTex,TRANSFORM_TEX(node_188.rg, _MainTex)).rgb,_Color.rgb,node_20),node_20);
                float3 emissive = (UNITY_LIGHTMODEL_AMBIENT.rgb*node_18);
                float3 finalColor = emissive + ((LightClamp( max(0,dot(lightDirection,i.normalDir)) )*node_18)*_LightColor0.rgb*attenuation);
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
            uniform sampler2D _Control; uniform float4 _Control_ST;
            uniform sampler2D _Splat0; uniform float4 _Splat0_ST;
            uniform sampler2D _Splat1; uniform float4 _Splat1_ST;
            uniform sampler2D _Splat2; uniform float4 _Splat2_ST;
            uniform sampler2D _Splat3; uniform float4 _Splat3_ST;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _Color;
            uniform sampler2D _Normal0; uniform float4 _Normal0_ST;
            uniform sampler2D _Normal1; uniform float4 _Normal1_ST;
            uniform sampler2D _Normal2; uniform float4 _Normal2_ST;
            uniform sampler2D _Normal3; uniform float4 _Normal3_ST;
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
            
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 binormalDir : TEXCOORD4;
                LIGHTING_COORDS(5,6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
/////// Normals:
                float2 node_189 = i.uv0;
                float4 node_7 = tex2D(_Control,TRANSFORM_TEX(node_189.rg, _Control));
                float4 node_12 = float4(node_7.rgb,node_7.a);
                float3 normalLocal = (node_12.r*tex2D(_Normal0,TRANSFORM_TEX(node_189.rg, _Normal0)).rgb + node_12.g*tex2D(_Normal1,TRANSFORM_TEX(node_189.rg, _Normal1)).rgb + node_12.b*tex2D(_Normal2,TRANSFORM_TEX(node_189.rg, _Normal2)).rgb + node_12.a*tex2D(_Normal3,TRANSFORM_TEX(node_189.rg, _Normal3)).rgb);
                float3 normalDirection =  normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float node_20 = 0.0;
                float3 node_18 = lerp((node_12.r*tex2D(_Splat0,TRANSFORM_TEX(node_189.rg, _Splat0)).rgb + node_12.g*tex2D(_Splat1,TRANSFORM_TEX(node_189.rg, _Splat1)).rgb + node_12.b*tex2D(_Splat2,TRANSFORM_TEX(node_189.rg, _Splat2)).rgb + node_12.a*tex2D(_Splat3,TRANSFORM_TEX(node_189.rg, _Splat3)).rgb),lerp(tex2D(_MainTex,TRANSFORM_TEX(node_189.rg, _MainTex)).rgb,_Color.rgb,node_20),node_20);
                float3 finalColor = ((LightClamp( max(0,dot(lightDirection,i.normalDir)) )*node_18)*_LightColor0.rgb*attenuation);
/// Final Color:
                return fixed4(finalColor * 1,0);
            }
            ENDCG
        }
    }

    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
