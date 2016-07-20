Shader "Custom/testShader" {
	Properties{

		_MainTex("Texture", 2D) = "white" {}
		_Ramp("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_FadeAlpha("Fade Alpha", Range(0,1)) = 1
		_FadeThreshold("Fade Threshold", float) = 0.5
		_FadePoint("Fade Point", Vector) = (0,0,0)
		_FadeAxis("Fade Axis", Vector) = (0,0,0)
	}
		SubShader{
			//Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" }
			Tags { "RenderType" = "Opaque"}
			LOD 200
			Cull Off

			CGPROGRAM
			#pragma surface surf Standard alpha:fade fullforwardshadows

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			sampler2D	_MainTex;
			fixed4		_Color;
			fixed		_FadeAlpha;
			float		_FadeThreshold;
			float3		_FadePoint;
			fixed3		_FadeAxis;


			struct Input {
				float3 worldPos;
				float2 uv_MainTex;
				float4 color : COLOR;
			};

			void surf(Input IN, inout SurfaceOutputStandard o) {

				//Set the texture as usual
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				o.Albedo = c.rgb * IN.color.rgb;

				//Make sure each axis is either 0 or 1
				_FadeAxis.x = saturate(ceil(_FadeAxis.x));
				_FadeAxis.y = saturate(ceil(_FadeAxis.y));
				_FadeAxis.z = saturate(ceil(_FadeAxis.z));

				//Discard axis that won't be used
				IN.worldPos.x *= _FadeAxis.x;
				IN.worldPos.y *= _FadeAxis.y;
				IN.worldPos.z *= _FadeAxis.z;
				_FadePoint.x *=  _FadeAxis.x;
				_FadePoint.y *=  _FadeAxis.y;
				_FadePoint.z *=  _FadeAxis.z;

				//Make sure the FadeThreshold is not less than 0 (kinda... damn precision errors)
				_FadeThreshold = max(0.00001, _FadeThreshold);

				//calculate the squared distance between the pixel and the fade point (so the gradient is not as spread out)
				float dist = pow(distance(IN.worldPos, _FadePoint), 2);

				//if no fade axis is set, then just make distance be farther than threshold
				dist += step(_FadeAxis.x + _FadeAxis.y + _FadeAxis.z, 0) * (_FadeThreshold + 1);

				//calculate how close to the fade point the pixel is (starting from the threshold)
				float percentage = (dist / _FadeThreshold);			

				float alpha = saturate(max(_FadeAlpha, percentage));
				o.Alpha = c.a * alpha;

			}

			ENDCG
		}
		FallBack "VertexLit"
}
