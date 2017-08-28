// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// Unlit shader. Simplest possible colored shader.
// - no lighting
// - no lightmap support
// - no texture

// This is a modified version that uses the stencil buffer to prevent overlapping. 
// This means that alpha values will not stack, and the objects will have a single, 
// consistent, transparent color.

Shader "Unlit/Transparent Color" {
	Properties {
	    _Color ("Main Color", Color) = (1,1,1,1)
	}

	SubShader {    
	    Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	    LOD 100

	    Blend SrcAlpha OneMinusSrcAlpha // perform alpha blending
	    ZWrite Off // do not write to the z buffer
     
	    Pass {

            // This chunk ensures that no pixel is rendered to if
            // it already contains a pixel.
	        Stencil
            {
                Ref 1 // using this value...
                Comp Greater // only render if 'Ref' is greater than the current value in the buffer
                Pass Replace // set 'Ref' into the stencil buffer; this means that no
                             // more drawing of this material will happen on this pixel,
                             // which means that transparent colours will not overlap
            }

	        CGPROGRAM
	            #pragma vertex vert // use 'vert' as the vertex function
	            #pragma fragment frag // use 'frag' as the fragment function
	            #pragma target 2.0 // target shader model 2.0
	            #pragma multi_compile_fog // generate multiple versions to support fog

	            #include "UnityCG.cginc"

	            struct appdata_t {
	                float4 vertex : POSITION;
	                UNITY_VERTEX_INPUT_INSTANCE_ID
	            };

	            struct v2f {
	                float4 vertex : SV_POSITION;
	                UNITY_FOG_COORDS(0)
	                UNITY_VERTEX_OUTPUT_STEREO
	            };

	            fixed4 _Color;

	            v2f vert (appdata_t v)
	            {
	                v2f o;
	                UNITY_SETUP_INSTANCE_ID(v);
	                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
	                o.vertex = UnityObjectToClipPos(v.vertex);
	                UNITY_TRANSFER_FOG(o,o.vertex);
	                return o;
	            }

	            fixed4 frag (v2f i) : COLOR
	            {
	                fixed4 col = _Color;
	                UNITY_APPLY_FOG(i.fogCoord, col);
	                return col;
	            }
	        ENDCG
	    }
	}

}
