// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "BlendVertexColorWithUV"
{
	Properties
	{
		_MainTex("Arrow texture", 2D) = "white" {}
		_FadeInTex("Fade in texture", 2D) = "gray" {}
		_FadeOutTex("Fade out texture", 2D) = "gray" {}
		_CurrentColor("Current color", Color) = (1, 1, 1, 1)
		_FillTex("Dummy texture to grab original UV coords", 2D) = "white" {}
		_FillColor("Fill color", Color) = (1, 0, 0, 1)
		_Offset("Color offset", Range(0, 1)) = 0.5
	}

	SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		ZWrite Off Lighting Off Cull Off Fog{ Mode Off } Blend SrcAlpha OneMinusSrcAlpha
		LOD 110

		Pass
		{
			CGPROGRAM
			#pragma vertex vert_vct
			#pragma fragment frag_mult 
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;

			sampler2D _FadeInTex;
			float4 _FadeInTex_ST;

			sampler2D _FadeOutTex;
			float4 _FadeOutTex_ST;

			sampler2D _FillTex;
			float4 _FillTex_ST;

			float4 _CurrentColor;
			float4 _FillColor;
			float _Offset;

			struct vin_vct
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				float2 fadeInTexcoord : TEXCOORD1;
				float2 fadeOutTexcoord : TEXCOORD2;
				float2 fillTexcoord : TEXCOORD3;
			};

			struct v2f_vct
			{
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				float2 fadeInTexcoord : TEXCOORD1;
				float2 fadeOutTexcoord : TEXCOORD2;
				float2 fillTexcoord : TEXCOORD3;
			};

			v2f_vct vert_vct(vin_vct v)
			{
				v2f_vct o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.fadeInTexcoord = TRANSFORM_TEX(v.fadeInTexcoord, _FadeInTex);
				o.fadeOutTexcoord = TRANSFORM_TEX(v.fadeOutTexcoord, _FadeOutTex);
				o.fillTexcoord = v.fillTexcoord;
				return o;
			}

			fixed4 frag_mult(v2f_vct i) : COLOR
			{
				fixed4 fadeInColor = tex2D(_FadeInTex, i.fadeInTexcoord);
				fixed4 fadeOutColor = tex2D(_FadeOutTex, i.fadeOutTexcoord);
				fixed4 fillColor = i.fillTexcoord.x <= _Offset ? _FillColor : _CurrentColor;
				fixed4 col = tex2D(_MainTex, i.texcoord) * i.color;
				
				return col * fadeInColor * fadeOutColor * fillColor;
			}
			ENDCG
		}
	}
}