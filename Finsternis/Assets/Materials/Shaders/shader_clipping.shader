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
			Cull Off
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
			};

			half _Glossiness;
			half _Metallic;
			fixed4 _Emission;
			fixed4 _Color;
			float _MaxY;
			float _FadeDstThreshold;

			void surf(Input IN, inout SurfaceOutputStandard o) {
				if (IN.worldPos.y > _MaxY || IN.worldPos.y < -0.1) discard;
				// Albedo comes from a texture tinted by color
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

				float diff = (_MaxY - IN.worldPos.y);
				if (diff < _FadeDstThreshold) {
					diff /= _FadeDstThreshold;
					c *= pow(lerp(1, 0, 1-diff),2);
				}

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
