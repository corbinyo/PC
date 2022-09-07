
Shader "Glass/GlassOpaque" {
    Properties {
        _ChromeColor ("ChromeColor", Color) = (0.3382353,0.3382353,0.3382353,1)
        _GlassSize ("GlassSize", Range(0, 10)) = 2.3
        _SpecularOpacity ("SpecularOpacity", Range(0, 1)) = 0.5
        _BlackLineSize ("BlackLineSize", Range(0, 1)) = 0.4
        _BlackLineTransparency ("BlackLineTransparency", Range(0, 1)) = 0.9
        _NormalMap ("NormalMap", 2D) = "bump" {}
        _RefractionIntensity ("RefractionIntensity", Range(-0.3, 0.3)) = -0.1
        _DustMap ("DustMap", 2D) = "black" {}
        _DustFadeDistance ("DustFadeDistance", Range(0, 3)) = 0.4
        _DustMaxIntensity ("DustMaxIntensity", Range(0, 1)) = 0.4
        _AdditionalGlossSize ("AdditionalGlossSize", Range(0, 1)) = 1
        _InternalLightSize ("InternalLightSize", Range(0, 1)) = 0.6
        _InternalLightIntensity ("InternalLightIntensity", Range(0, 1)) = 0.2
        _LightTransmission ("LightTransmission", Range(0, 1)) = 0.2
        _LightRefraction ("LightRefraction", Range(0, 1)) = 0.5
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "Queue"="Transparent"
            "RenderType"="Opaque"
        }
        GrabPass{ }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
            #pragma target 2.0
            uniform sampler2D _GrabTexture;
            uniform float _RefractionIntensity;
            uniform sampler2D _DustMap; uniform float4 _DustMap_ST;
            uniform sampler2D _NormalMap; uniform float4 _NormalMap_ST;
            uniform float _BlackLineSize;
            uniform float _BlackLineTransparency;
            uniform float _GlassSize;
            uniform float _AdditionalGlossSize;
            uniform float _DustMaxIntensity;
            uniform float _SpecularOpacity;
            uniform float _DustFadeDistance;
            uniform float4 _ChromeColor;
            uniform float _InternalLightSize;
            uniform float _InternalLightIntensity;
            uniform float _LightTransmission;
            uniform float _LightRefraction;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                float4 screenPos : TEXCOORD7;
                LIGHTING_COORDS(8,9)
                UNITY_FOG_COORDS(10)
                #if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
                    float4 ambientOrLightmapUV : TEXCOORD11;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                #ifdef LIGHTMAP_ON
                    o.ambientOrLightmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                    o.ambientOrLightmapUV.zw = 0;
                #endif
                #ifdef DYNAMICLIGHTMAP_ON
                    o.ambientOrLightmapUV.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
                #endif
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.screenPos = o.pos;
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                i.normalDir = normalize(i.normalDir);
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _NormalMap_var = UnpackNormal(tex2D(_NormalMap,TRANSFORM_TEX(i.uv0, _NormalMap)));
                float3 normal = _NormalMap_var.rgb;
                float3 normalLocal = normal;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float v_refractionIntensity = _RefractionIntensity;
                float2 refraction = (v_refractionIntensity*mul( UNITY_MATRIX_V, float4(normalDirection,0) ).xyz.rgb.rg);
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + refraction;
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float Gloss = 0.95;
                float gloss = Gloss;
                float perceptualRoughness = 1.0 - Gloss;
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
/////// GI Data:
                UnityLight light;
                #ifdef LIGHTMAP_OFF
                    light.color = lightColor;
                    light.dir = lightDirection;
                    light.ndotl = LambertTerm (normalDirection, light.dir);
                #else
                    light.color = half3(0.f, 0.f, 0.f);
                    light.ndotl = 0.0f;
                    light.dir = half3(0.f, 0.f, 0.f);
                #endif
                UnityGIInput d;
                d.light = light;
                d.worldPos = i.posWorld.xyz;
                d.worldViewDir = viewDirection;
                d.atten = attenuation;
                #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
                    d.ambient = 0;
                    d.lightmapUV = i.ambientOrLightmapUV;
                #else
                    d.ambient = i.ambientOrLightmapUV;
                #endif
                #if UNITY_SPECCUBE_BLENDING || UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMin[0] = unity_SpecCube0_BoxMin;
                    d.boxMin[1] = unity_SpecCube1_BoxMin;
                #endif
                #if UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMax[0] = unity_SpecCube0_BoxMax;
                    d.boxMax[1] = unity_SpecCube1_BoxMax;
                    d.probePosition[0] = unity_SpecCube0_ProbePosition;
                    d.probePosition[1] = unity_SpecCube1_ProbePosition;
                #endif
                d.probeHDR[0] = unity_SpecCube0_HDR;
                d.probeHDR[1] = unity_SpecCube1_HDR;
                Unity_GlossyEnvironmentData ugls_en_data;
                ugls_en_data.roughness = 1.0 - gloss;
                ugls_en_data.reflUVW = viewReflectDirection;
                UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );
                lightDirection = gi.light.dir;
                lightColor = gi.light.color;
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float node_7142 = (1.0-max(0,dot(normalDirection, viewDirection)));
                float v_glassSize = _GlassSize;
                float node_138 = v_glassSize;
                float v_blackLineSize = _BlackLineSize;
                float node_5591 = 50.0;
                float node_2232 = saturate(pow((node_7142*(node_138+(node_138*v_blackLineSize))),node_5591));
                float node_7792 = saturate(pow((node_138*node_7142),node_5591));
                float blackline = saturate((node_2232-node_7792));
                float v_blackLineTransparency = _BlackLineTransparency;
                float speculareambiantocclusion = saturate(((1.0 - blackline)+(v_blackLineTransparency*-1.0+1.0)));
                float3 specularAO = speculareambiantocclusion;
                float v_lightRefraction = _LightRefraction;
                float3 bitangentLight = saturate((v_lightRefraction*_LightColor0.rgb*pow(((1.0 - max(0,dot(viewDirection,viewReflectDirection)))*max(0,dot(i.tangentDir,normalDirection))*5.0),2.0)*20.0));
                float node_4869 = pow(max(0,dot((-1*lightDirection),normalDirection)),1.0);
                float node_3274 = (1.0-max(0,dot(normalDirection, viewDirection)));
                float v_lightTransmission = _LightTransmission;
                float v_additionalGlossSize = _AdditionalGlossSize;
                float insideLine = (1.0 - node_2232);
                float v_internalLightIntensity = _InternalLightIntensity;
                float v_internalLightSize = _InternalLightSize;
                float lightLine = saturate((saturate(pow((node_7142*(node_138+0.5)),(v_internalLightSize*-5.0+5.0)))-node_2232));
                float3 insideLight = ((saturate((((saturate((saturate((10.0*node_4869*node_3274))+(saturate(((node_3274+5.0)*node_4869*1.0))*0.4)))*v_lightTransmission*3.0)+(pow(max(0,dot(lightDirection,normalDirection)),5.0)*pow(max(0,dot(normalDirection,viewReflectDirection)),500.0)*500.0*v_additionalGlossSize))*insideLine))+(v_internalLightIntensity*5.0*lightLine))*_LightColor0.rgb);
                float4 _DustMap_var = tex2D(_DustMap,TRANSFORM_TEX(i.uv0, _DustMap));
                float v_dustMaxIntensity = _DustMaxIntensity;
                float v_dustFadeDistance = _DustFadeDistance;
                float3 dust = (_DustMap_var.rgb*v_dustMaxIntensity*100.0*saturate((v_dustFadeDistance/distance(_WorldSpaceCameraPos,i.posWorld.rgb))));
                float3 ChromeColor = _ChromeColor.rgb;
                float3 speculareambiantlight = (bitangentLight+insideLight+pow((dust*_LightColor0.rgb*attenuation*2.0),1.5)+(_LightColor0.rgb*attenuation*ChromeColor*0.6));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float spec = 0.9;
                float node_400 = spec;
                float3 specularColor = float3(node_400,node_400,node_400);
                float specularMonochrome;
                float node_7230 = 0.0;
                float3 diffuseColor = float3(node_7230,node_7230,node_7230); // Need this for specular when using metallic
                diffuseColor = EnergyConservationBetweenDiffuseAndSpecular(diffuseColor, specularColor, specularMonochrome);
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
                float VdotH = saturate(dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                float normTerm = GGXTerm(NdotH, roughness);
                float specularPBL = (visTerm*normTerm) * UNITY_PI;
                #ifdef UNITY_COLORSPACE_GAMMA
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                #endif
                specularPBL = max(0, specularPBL * NdotL);
                #if defined(_SPECULARHIGHLIGHTS_OFF)
                    specularPBL = 0.0;
                #endif
                half surfaceReduction;
                #ifdef UNITY_COLORSPACE_GAMMA
                    surfaceReduction = 1.0-0.28*roughness*perceptualRoughness;
                #else
                    surfaceReduction = 1.0/(roughness*roughness + 1.0);
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                half grazingTerm = saturate( gloss + specularMonochrome );
                float3 indirectSpecular = (gi.indirect.specular + speculareambiantlight) * specularAO;
                indirectSpecular *= FresnelLerp (specularColor, grazingTerm, NdotV);
                indirectSpecular *= surfaceReduction;
                float3 specular = (directSpecular + indirectSpecular);
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += gi.indirect.diffuse;
                diffuseColor *= 1-specularMonochrome;
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                float v_specularOpacity = _SpecularOpacity;
                float3 node_3880 = ChromeColor;
                float4 node_2111_k = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                float4 node_2111_p = lerp(float4(float4(node_3880,0.0).zy, node_2111_k.wz), float4(float4(node_3880,0.0).yz, node_2111_k.xy), step(float4(node_3880,0.0).z, float4(node_3880,0.0).y));
                float4 node_2111_q = lerp(float4(node_2111_p.xyw, float4(node_3880,0.0).x), float4(float4(node_3880,0.0).x, node_2111_p.yzx), step(node_2111_p.x, float4(node_3880,0.0).x));
                float node_2111_d = node_2111_q.x - min(node_2111_q.w, node_2111_q.y);
                float node_2111_e = 1.0e-10;
                float3 node_2111 = float3(abs(node_2111_q.z + (node_2111_q.w - node_2111_q.y) / (6.0 * node_2111_d + node_2111_e)), node_2111_d / (node_2111_q.x + node_2111_e), node_2111_q.x);;
                float Opacity = saturate((saturate((saturate(dust).r+saturate(((insideLight/5.0)+saturate(((speculareambiantlight+(1.0 - insideLine))*v_specularOpacity))+blackline)).r))+(node_2111.g/1.5)));
                fixed4 finalRGBA = fixed4(lerp(sceneColor.rgb, finalColor,Opacity),1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
            #pragma target 2.0
            uniform sampler2D _GrabTexture;
            uniform float _RefractionIntensity;
            uniform sampler2D _DustMap; uniform float4 _DustMap_ST;
            uniform sampler2D _NormalMap; uniform float4 _NormalMap_ST;
            uniform float _BlackLineSize;
            uniform float _GlassSize;
            uniform float _AdditionalGlossSize;
            uniform float _DustMaxIntensity;
            uniform float _SpecularOpacity;
            uniform float _DustFadeDistance;
            uniform float4 _ChromeColor;
            uniform float _InternalLightSize;
            uniform float _InternalLightIntensity;
            uniform float _LightTransmission;
            uniform float _LightRefraction;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                float4 screenPos : TEXCOORD7;
                LIGHTING_COORDS(8,9)
                UNITY_FOG_COORDS(10)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.screenPos = o.pos;
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                i.normalDir = normalize(i.normalDir);
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _NormalMap_var = UnpackNormal(tex2D(_NormalMap,TRANSFORM_TEX(i.uv0, _NormalMap)));
                float3 normal = _NormalMap_var.rgb;
                float3 normalLocal = normal;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float v_refractionIntensity = _RefractionIntensity;
                float2 refraction = (v_refractionIntensity*mul( UNITY_MATRIX_V, float4(normalDirection,0) ).xyz.rgb.rg);
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + refraction;
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float Gloss = 0.95;
                float gloss = Gloss;
                float perceptualRoughness = 1.0 - Gloss;
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float spec = 0.9;
                float node_400 = spec;
                float3 specularColor = float3(node_400,node_400,node_400);
                float specularMonochrome;
                float node_7230 = 0.0;
                float3 diffuseColor = float3(node_7230,node_7230,node_7230); // Need this for specular when using metallic
                diffuseColor = EnergyConservationBetweenDiffuseAndSpecular(diffuseColor, specularColor, specularMonochrome);
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
                float VdotH = saturate(dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                float normTerm = GGXTerm(NdotH, roughness);
                float specularPBL = (visTerm*normTerm) * UNITY_PI;
                #ifdef UNITY_COLORSPACE_GAMMA
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                #endif
                specularPBL = max(0, specularPBL * NdotL);
                #if defined(_SPECULARHIGHLIGHTS_OFF)
                    specularPBL = 0.0;
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                diffuseColor *= 1-specularMonochrome;
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                float4 _DustMap_var = tex2D(_DustMap,TRANSFORM_TEX(i.uv0, _DustMap));
                float v_dustMaxIntensity = _DustMaxIntensity;
                float v_dustFadeDistance = _DustFadeDistance;
                float3 dust = (_DustMap_var.rgb*v_dustMaxIntensity*100.0*saturate((v_dustFadeDistance/distance(_WorldSpaceCameraPos,i.posWorld.rgb))));
                float node_4869 = pow(max(0,dot((-1*lightDirection),normalDirection)),1.0);
                float node_3274 = (1.0-max(0,dot(normalDirection, viewDirection)));
                float v_lightTransmission = _LightTransmission;
                float v_additionalGlossSize = _AdditionalGlossSize;
                float node_7142 = (1.0-max(0,dot(normalDirection, viewDirection)));
                float v_glassSize = _GlassSize;
                float node_138 = v_glassSize;
                float v_blackLineSize = _BlackLineSize;
                float node_5591 = 50.0;
                float node_2232 = saturate(pow((node_7142*(node_138+(node_138*v_blackLineSize))),node_5591));
                float insideLine = (1.0 - node_2232);
                float v_internalLightIntensity = _InternalLightIntensity;
                float v_internalLightSize = _InternalLightSize;
                float lightLine = saturate((saturate(pow((node_7142*(node_138+0.5)),(v_internalLightSize*-5.0+5.0)))-node_2232));
                float3 insideLight = ((saturate((((saturate((saturate((10.0*node_4869*node_3274))+(saturate(((node_3274+5.0)*node_4869*1.0))*0.4)))*v_lightTransmission*3.0)+(pow(max(0,dot(lightDirection,normalDirection)),5.0)*pow(max(0,dot(normalDirection,viewReflectDirection)),500.0)*500.0*v_additionalGlossSize))*insideLine))+(v_internalLightIntensity*5.0*lightLine))*_LightColor0.rgb);
                float v_lightRefraction = _LightRefraction;
                float3 bitangentLight = saturate((v_lightRefraction*_LightColor0.rgb*pow(((1.0 - max(0,dot(viewDirection,viewReflectDirection)))*max(0,dot(i.tangentDir,normalDirection))*5.0),2.0)*20.0));
                float3 ChromeColor = _ChromeColor.rgb;
                float3 speculareambiantlight = (bitangentLight+insideLight+pow((dust*_LightColor0.rgb*attenuation*2.0),1.5)+(_LightColor0.rgb*attenuation*ChromeColor*0.6));
                float v_specularOpacity = _SpecularOpacity;
                float node_7792 = saturate(pow((node_138*node_7142),node_5591));
                float blackline = saturate((node_2232-node_7792));
                float3 node_3880 = ChromeColor;
                float4 node_2111_k = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                float4 node_2111_p = lerp(float4(float4(node_3880,0.0).zy, node_2111_k.wz), float4(float4(node_3880,0.0).yz, node_2111_k.xy), step(float4(node_3880,0.0).z, float4(node_3880,0.0).y));
                float4 node_2111_q = lerp(float4(node_2111_p.xyw, float4(node_3880,0.0).x), float4(float4(node_3880,0.0).x, node_2111_p.yzx), step(node_2111_p.x, float4(node_3880,0.0).x));
                float node_2111_d = node_2111_q.x - min(node_2111_q.w, node_2111_q.y);
                float node_2111_e = 1.0e-10;
                float3 node_2111 = float3(abs(node_2111_q.z + (node_2111_q.w - node_2111_q.y) / (6.0 * node_2111_d + node_2111_e)), node_2111_d / (node_2111_q.x + node_2111_e), node_2111_q.x);;
                float Opacity = saturate((saturate((saturate(dust).r+saturate(((insideLight/5.0)+saturate(((speculareambiantlight+(1.0 - insideLine))*v_specularOpacity))+blackline)).r))+(node_2111.g/1.5)));
                fixed4 finalRGBA = fixed4(finalColor * Opacity,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "Meta"
            Tags {
                "LightMode"="Meta"
            }
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_META 1
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "UnityMetaPass.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
            #pragma target 2.0
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv1 : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
                float4 posWorld : TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
                return o;
            }
            float4 frag(VertexOutput i) : SV_Target {
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                UnityMetaInput o;
                UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
                
                o.Emission = 0;
                
                float node_7230 = 0.0;
                float3 diffColor = float3(node_7230,node_7230,node_7230);
                float spec = 0.9;
                float node_400 = spec;
                float3 specColor = float3(node_400,node_400,node_400);
                float specularMonochrome = max(max(specColor.r, specColor.g),specColor.b);
                diffColor *= (1.0-specularMonochrome);
                float Gloss = 0.95;
                float roughness = 1.0 - Gloss;
                o.Albedo = diffColor + specColor * roughness * roughness * 0.5;
                
                return UnityMetaFragment( o );
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
