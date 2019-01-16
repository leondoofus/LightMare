// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/RayDiv" {
	Properties
    {
        // Color property for material inspector, default to white
        _Color ("Main Color", Color) = (1,1,1,1)
    }
    SubShader
    {
		Cull Off
		Blend One One
		Lighting Off
        ZWrite Off
        Fog { Mode Off }
		//Blend One OneMinusSrcAlpha

		 Tags
        { 
            "Queue"="Transparent" 
			"RenderType"="Transparent" 

           "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            // vertex shader
            // this time instead of using "appdata" struct, just spell inputs manually,
            // and instead of returning v2f struct, also just return a single output
            // float4 clip position
            struct appdata
            {
                float4 vertex : POSITION; // vertex position
                float2 uv : TEXCOORD0; // texture coordinate
				//float4 color    : COLOR;
            };

            // vertex shader outputs ("vertex to fragment")
            struct v2f
            {
                float2 uv : TEXCOORD0; // texture coordinate
                float4 vertex : SV_POSITION; // clip space position
				//fixed4 color    : COLOR;
            };

            // vertex shader
            v2f vert (appdata v)
            {
                v2f o;
                // transform position to clip space
                // (multiply with model*view*projection matrix)
                o.vertex = UnityObjectToClipPos(v.vertex);
                // just pass the texture coordinate
                o.uv = v.uv;
				//o.color=v.color;
                return o;
            }
            
            // color from the material
            fixed4 _Color;

            // pixel shader
            fixed4 frag (v2f i) : SV_Target
            {
				float4 c=_Color;
				float a=i.uv.x ;
				if(a<0.1f) a=0.1f;
				c.rgb*=c.a / a;
                return c; // just return it
            }
            ENDCG
        }
    }
}
