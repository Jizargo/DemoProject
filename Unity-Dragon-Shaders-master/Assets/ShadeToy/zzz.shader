Shader "东方喵/可爱的GrabPass"   
{  
	Properties
	{
	_NormalMap("法线图", 2D) = "bump" {}
	}

    SubShader  
    {  
        ZWrite Off   
        GrabPass  
        {    
            "_GrabTex"//【注1】
        }  
  
        Pass  
        {  
            Tags  
            {   
                "RenderType" = "Transparent"  
                "Queue" = "Transparent+1"  
            }  
  
            CGPROGRAM      
            #include "UnityCG.cginc"  
			sampler2D _NormalMap;
			float4 _GrabTex_TexelSize;//抓屏贴图的纹素大小
			sampler2D _GrabTex;//抓屏贴图
				

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};
			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 uv0 : TEXCOORD1;
			};
  
			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);//【注2】
				o.uv = v.uv;
				o.uv0 = ComputeGrabScreenPos(o.pos);//【注3】
				return o;
			}
  
            fixed4 frag(v2f i) : SV_Target  
            {  
				float3 bump = UnpackNormal(tex2D(_NormalMap, i.uv));//【注4】
				float2 offset = bump.rg * _GrabTex_TexelSize.rg *25* sin(_Time.g);
				fixed4 o = tex2D(_GrabTex, (i.uv0.rg + offset) / i.uv0.a);
				return o-fixed4(0.1,0.1,0.1,0);
            }  
            #pragma vertex vert  
            #pragma fragment frag  
            ENDCG  
        }  
    }  
}  