Shader "Custom/CrystalBall" {
	Properties{
		_Tex1("MainTex", 2D) = "white" {}//主图
	    _Tex2("HighLight", 2D) = "white" {}//假高光
		_Tex3("NoiseMap",2D) = "white"{}//噪音图
	}
SubShader{
	Tags{ "RenderType" = "Transparent" "Queue" = "Transparent" }
		LOD 200
		Cull off
		CGPROGRAM
#pragma surface surf Lambert alpha:blend
	sampler2D _Tex1;
	sampler2D _Tex2;
	sampler2D _Tex3;

	struct Input {
		float2 uv_Tex1;
	};

	void surf(Input IN, inout SurfaceOutput o) {
		half4 c1;
		half4 c2;
		half4 c3;
		float4 gray;

		float temp;//(x-a)^2+(y-b)^2=r^2
		const float center =0.5;//球心
		const float r = 0.51;//半径(180度广角)
		float2 uvTemp;
		float2 uvNoise;
		float t;
		float xx;
		float yy;
		float alpha;

		xx = IN.uv_Tex1.r;
		yy = center + r*(IN.uv_Tex1.g - center) / (sqrt(pow(r, 2) - pow(IN.uv_Tex1.r - center, 2)));
		t = _Time.y/10;
		xx = xx+0.01*sin(IN.uv_Tex1.g*10+t*10);
		uvNoise= float2(IN.uv_Tex1.r,IN.uv_Tex1.g + 0.08*sin(IN.uv_Tex1.g * 4 + t * 4));
		alpha=

		uvTemp = float2(xx,yy);
		c1 = tex2D(_Tex1,uvTemp);
		c2 = tex2D(_Tex2, IN.uv_Tex1);
		c3 = tex2D(_Tex3,uvNoise);

		temp = (IN.uv_Tex1.r*IN.uv_Tex1.r)+(IN.uv_Tex1.g*IN.uv_Tex1.g)-IN.uv_Tex1.r-IN.uv_Tex1.g;
		if (temp>-0.25) {
			o.Alpha = 0;
		}
		else {
			gray = float4((c3.r*0.3+c3.g*0.59+c3.b*0.11)*0.54,(c3.r*0.3 + c3.g*0.59 + c3.b*0.11)*0.17,(c3.r*0.3 + c3.g*0.59 + c3.b*0.11)*0.89,1);
			o.Emission = c1.rgb*0.5 + c2.rgb*0.8 +0.5*gray.rgb;
			o.Alpha = 1;
		}
	}
	ENDCG
	}
	FallBack off 
}

