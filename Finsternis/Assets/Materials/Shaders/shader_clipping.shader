Shader "Custom/shader_clipping" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Normal("Normal", 2D) = "bump" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_Emission("Emission", Color) = (0,0,0,0)

		_MaxY("Maximum Y", Range(0,100)) = 10
		_FadeDstThreshold("Fade distance from Y", Range(0,1)) = 0.5
	}
		SubShader{
			Tags { "RenderType" = "Opaque" }
			Cull Off //make shader 2 sided
			LOD 200

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard fullforwardshadows

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			sampler2D _MainTex;

			struct Input {
				float3 worldPos;
				float2 uv_MainTex;
				float3 worldNormal;
				float3 viewDir;
			};

			half _Glossiness;
			half _Metallic;
			fixed4 _Emission;
			fixed4 _Color;
			float _MaxY;
			float _FadeDstThreshold;

			void surf(Input IN, inout SurfaceOutputStandard o) {

				//Discard fragments with y below 0.01 and above _MaxY
				clip(IN.worldPos.y + 0.01);
				clip(_MaxY - IN.worldPos.y);

				// Albedo comes from a texture tinted by color
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

				//Gently fade to black as the fragments get closer to _MaxY
				float diff = clamp((_MaxY - IN.worldPos.y), 0, _FadeDstThreshold);
				diff /= _FadeDstThreshold;
				c *= sqrt(lerp(1, 0, 1 - diff));

				c *= ceil(dot(IN.viewDir, IN.worldNormal)); //make back faces black

				o.Albedo = c.rgb;
				// Metallic and smoothness come from slider variables
				o.Metallic = _Metallic;
				o.Smoothness = _Glossiness;
				o.Emission = _Emission;
				o.Alpha = c.a;
			}
			ENDCG
		}
			FallBack "Diffuse"
}
