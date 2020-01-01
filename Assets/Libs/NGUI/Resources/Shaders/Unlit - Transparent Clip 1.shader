// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/Unlit/Transparent Clip 1"  
{  
    Properties  
    {  
        _MainTex ("Base (RGB), Alpha (A)", 2D) = "black" {}  
    }  
  
    SubShader  
    {  
        LOD 200  
  
        Tags  
        {  
            "Queue" = "Transparent"  
            "IgnoreProjector" = "True"  
            "RenderType" = "Transparent"  
        }  
          
        Pass  
        {  
            Cull Off  
            Lighting Off  
            ZWrite Off  
            Offset -1, -1  
            Fog { Mode Off }  
            ColorMask RGB  
            AlphaTest Greater .01  
            Blend SrcAlpha OneMinusSrcAlpha  
  
            CGPROGRAM  
            #pragma vertex vert  
            #pragma fragment frag  
  
            #include "UnityCG.cginc"  
  
            sampler2D _MainTex;  
            float2 _ClipArgs0 = float2(1000.0, 1000.0);  
  
			float2 _PanelSize0 = float2(0.0, 0.0);
			float2 _PanelOffset0 = float2(0.0, 0.0);

            struct appdata_t  
            {  
                float4 vertex : POSITION;  
                half4 color : COLOR;  
                float2 texcoord : TEXCOORD0;  
            };  
  
            struct v2f  
            {  
                float4 vertex : POSITION;  
                half4 color : COLOR;  
                float2 texcoord : TEXCOORD0;  
                float2 worldPos : TEXCOORD1;  
            };  
  
            v2f vert (appdata_t v)  
            {  
                v2f o;  
                o.vertex = UnityObjectToClipPos(v.vertex);  
                o.color = v.color;  
                o.texcoord = v.texcoord;  
                //o.worldPos = v.vertex.xy * _ClipRange0.zw + _ClipRange0.xy;  

				float2 worldPos = o.vertex.xy / o.vertex.w;
				worldPos = (worldPos.xy + 1) * 0.5 - _PanelOffset0.xy;

				//worldPos.x = worldPos.x * 2 / _PanelSize0.x;
				//worldPos.y = worldPos.y * 2 / _PanelSize0.y;
				o.worldPos = worldPos.xy * 2 / _PanelSize0.xy ;
                return o;  
            }  
  
            half4 frag (v2f IN) : COLOR  
            {  
                // Softness factor  
                //float2 factor = (float2(1.0, 1.0) - abs(IN.worldPos)) * _ClipArgs0;
				float2 factor = (float2(1.0, 1.0) - abs(IN.worldPos)) ;
              
                // Sample the texture  
                half4 col = tex2D(_MainTex, IN.texcoord);  
  
                  
                if (dot(IN.color, fixed4(1,1,1,0)) == 0)  
                {  
                  col = tex2D(_MainTex, IN.texcoord);  
                  col.rgb = dot(col.rgb, fixed3(.222,.707,.071));  
				  //col = col * IN.color;
                }else{  
                  col = col * IN.color;  
                }  
				//col.a = 1;
                col.a *= clamp( min(factor.x, factor.y), 0.0, 1.0);  
                return col;  
            }  
            ENDCG  
        }  
    }  
      
    SubShader  
    {  
        LOD 100  
  
        Tags  
        {  
            "Queue" = "Transparent"  
            "IgnoreProjector" = "True"  
            "RenderType" = "Transparent"  
        }  
          
        Pass  
        {  
            Cull Off  
            Lighting Off  
            ZWrite Off  
            Fog { Mode Off }  
            ColorMask RGB  
            AlphaTest Greater .01  
            Blend SrcAlpha OneMinusSrcAlpha  
            ColorMaterial AmbientAndDiffuse  
              
            SetTexture [_MainTex]  
            {  
                Combine Texture * Primary  
            }  
        }  
    }  
}