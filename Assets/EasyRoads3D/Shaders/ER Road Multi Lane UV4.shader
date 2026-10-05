// © 2025 EasyRoads3D
// This is a standard road shader supporting multi-lane road type setup
// This type of material can be auto generated from: General Settings > Road Types > Road Type Profile Editor > Material Tab > Road Type Material Creator
Shader "EasyRoads3D/ER Road Multi Lane UV4"
{
	Properties
	{
		[Space]
		[Header(Base Maps Settings)]
		[Space]
		[Space]
		[NoScaleOffset]_Albedo("Albedo", 2D) = "white" {}
		_MainTiling("Tiling", Vector) = (1,1,0,0)
		_Color("Color", Color) = (0.8,0.8,0.8,1)
		[NoScaleOffset]_Normals("Normal Map", 2D) = "white" {}
		[NoScaleOffset]_Metallic("Metallic (R) AO (G) Smoothness (A)", 2D) = "white" {}
		[HideInInspector]_BaseOffset("Base Offset", Vector) = (0,0,0,0)
		[HideInInspector]_MinXIncoming("Min X Incoming", Range( 0 , 1)) = 0
		[HideInInspector]_MaxXIncoming("Max X Incoming", Range( 0 , 1)) = 1
		[HideInInspector]_MinXOutGoing("Min X Out Going", Range( 0 , 1)) = 0
		[HideInInspector]_MaxXOutGoing("Max X Out Going", Range( 0 , 1)) = 0.5
		[HideInInspector]_MinXIncomingLeftEdge("Min X Incoming Left Edge", Range( 0 , 1)) = 0
		[HideInInspector]_MaxXIncomingLeftEdge("Max X Incoming Left Edge", Range( 0 , 1)) = 0.05
		[HideInInspector]_MinXOutGoingLeftEdge("Min X Out Going Left Edge", Range( 0 , 1)) = 0.51
		[HideInInspector]_MaxXOutGoingLeftEdge("Max X Out Going Left Edge", Range( 0 , 1)) = 0.75
		[HideInInspector]_MinXIncomingRightEdge("Min X Incoming Right Edge", Range( 0 , 1)) = 0.95
		[HideInInspector]_MaxXIncomingRightEdge("Max X Incoming Right Edge", Range( 0 , 1)) = 1
		[HideInInspector]_MinXOutGoingRightEdge("Min X Out Going Right Edge", Range( 0 , 1)) = 0.751
		[HideInInspector]_MaxXOutGoingRightEdge("Max X Out Going Right Edge", Range( 0 , 1)) = 1
		_BumpScale1("Normal Map Scale", Range( 0 , 4)) = 1
		_MetallicPower("Metallic Power", Range( 0 , 2)) = 0
		_SmoothnessPower("Smoothness Power", Range( 0 , 2)) = 1
		_OcclusionStrength1("Ambient Occlusion Power", Range( 0 , 2)) = 1
		[Toggle]_ShoulderEdgeMapping("Shoulder / Edge Mapping", Float) = 0
		[Space]
		[Header(Line Marking Settings)]
		[Space]
		[Space]
		_Lines("Albedo", 2D) = "white" {}
		[NoScaleOffset]_LinesMask("Line Markings Mask", 2D) = "white" {}
		_LineMaskHeightThreshold("Mask Height Threshold", Range( 0 , 1)) = 1
		_LineMaskPower("Mask Power", Range( 0 , 1)) = 0.5
		_BumpScale3("Normal Map Power", Range( 0 , 1)) = 0.25
		[Space]
		[Header(Lane Settings)]
		[Space]
		[Space]
		[NoScaleOffset]_MetSMTyres("Metallic Smoothness Tyres", 2D) = "white" {}
		[NoScaleOffset]_MetSMTyresNoise("Metallic Smoothness Connections", 2D) = "white" {}
		_LanesMetallicPower("Metallic Power", Range( 0 , 1)) = 0
		_LanesSmoothnessPower("Smoothness Power ", Range( 0 , 1.15)) = 1
		[Space]
		[Header(Detail and Noise Settings)]
		[Space]
		[Space]
		_Detail("map", 2D) = "white" {}
		_DetailStrength("Strength", Range( 0 , 2)) = 1
		_DetailHeightThreshold("Height Threshold", Range( 0 , 1)) = 0.5
		_DetailThresholdBlend("Threshold Blend", Range( 0 , 1)) = 0.25
		
		[Space]
		[Space]
		[Header(Terrain Z Fighting Offset)]
		[Space]
		_OffsetFactor("Offset Factor", Range(0.0,-10.0)) = -1
		_OffsetUnit("Offset Unit", Range(0.0,-10.0)) = -1
		[HideInInspector]_MinRightEdgeUV("Min Right Edge UV", Float) = 0.95
		[HideInInspector]_MaxLeftEdgeUV("Max Left Edge UV", Float) = 0.05
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] _texcoord4( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+2450" "IgnoreProjector" = "True" }
		Offset  [_OffsetFactor] , [_OffsetUnit]
		LOD 200
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#pragma target 4.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv4_texcoord4;
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
		};

		uniform float _ShoulderEdgeMapping;
		uniform sampler2D _Normals;
		uniform half _BumpScale1;
		uniform float2 _MainTiling;
		uniform float2 _BaseOffset;
		uniform float _MinXIncoming;
		uniform float _MaxXIncoming;
		uniform float _MinXOutGoing;
		uniform float _MaxXOutGoing;
		uniform float _MinRightEdgeUV;
		uniform float _MinXIncomingRightEdge;
		uniform float _MaxXIncomingRightEdge;
		uniform float _MinXOutGoingRightEdge;
		uniform float _MaxXOutGoingRightEdge;
		uniform sampler2D _Albedo;
		uniform float _MaxLeftEdgeUV;
		uniform float _MinXIncomingLeftEdge;
		uniform float _MaxXIncomingLeftEdge;
		uniform float _MinXOutGoingLeftEdge;
		uniform float _MaxXOutGoingLeftEdge;
		uniform sampler2D _LinesMask;
		uniform sampler2D _Metallic;
		uniform float _LineMaskHeightThreshold;
		uniform float _LineMaskPower;
		uniform half _BumpScale3;
		uniform sampler2D _Detail;
		uniform float4 _Detail_ST;
		uniform float4 _Color;
		uniform half _DetailStrength;
		uniform half _DetailHeightThreshold;
		uniform half _DetailThresholdBlend;
		uniform sampler2D _Lines;
		uniform float4 _Lines_ST;
		uniform half _MetallicPower;
		uniform sampler2D _MetSMTyres;
		uniform sampler2D _MetSMTyresNoise;
		uniform half _LanesMetallicPower;
		uniform half _SmoothnessPower;
		uniform half _LanesSmoothnessPower;
		uniform half _OcclusionStrength1;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv4_TexCoord74 = i.uv4_texcoord4 * _MainTiling + _BaseOffset;
			float temp_output_146_0 = (_MinXOutGoing + (( uv4_TexCoord74.x - floor( uv4_TexCoord74.x ) ) - _MinXIncoming) * (_MaxXOutGoing - _MinXOutGoing) / (_MaxXIncoming - _MinXIncoming));
			float4 appendResult147 = (float4(temp_output_146_0 , uv4_TexCoord74.y , 0.0 , 0.0));
			float3 tex2DNode5 = UnpackScaleNormal( tex2D( _Normals, appendResult147.xy ), _BumpScale1 );
			float ifLocalVar196 = 0;
			if( i.uv_texcoord.x > _MinRightEdgeUV )
				ifLocalVar196 = (_MinXOutGoingRightEdge + (i.uv_texcoord.x - _MinXIncomingRightEdge) * (_MaxXOutGoingRightEdge - _MinXOutGoingRightEdge) / (_MaxXIncomingRightEdge - _MinXIncomingRightEdge));
			else if( i.uv_texcoord.x < _MinRightEdgeUV )
				ifLocalVar196 = temp_output_146_0;
			float2 uv_TexCoord234 = i.uv_texcoord * _MainTiling + _BaseOffset;
			float4 appendResult197 = (float4(ifLocalVar196 , uv_TexCoord234.y , 0.0 , 0.0));
			float4 tex2DNode198 = tex2D( _Albedo, appendResult197.xy );
			float3 lerpResult202 = lerp( tex2DNode5 , UnpackScaleNormal( tex2D( _Normals, appendResult197.xy ), _BumpScale1 ) , tex2DNode198.a);
			float3 ifLocalVar205 = 0;
			if( i.uv_texcoord.x > _MinRightEdgeUV )
				ifLocalVar205 = lerpResult202;
			else if( i.uv_texcoord.x < _MinRightEdgeUV )
				ifLocalVar205 = tex2DNode5;
			float ifLocalVar161 = 0;
			if( i.uv_texcoord.x > _MaxLeftEdgeUV )
				ifLocalVar161 = temp_output_146_0;
			else if( i.uv_texcoord.x < _MaxLeftEdgeUV )
				ifLocalVar161 = (_MinXOutGoingLeftEdge + (i.uv_texcoord.x - _MinXIncomingLeftEdge) * (_MaxXOutGoingLeftEdge - _MinXOutGoingLeftEdge) / (_MaxXIncomingLeftEdge - _MinXIncomingLeftEdge));
			float4 appendResult162 = (float4(ifLocalVar161 , uv_TexCoord234.y , 0.0 , 0.0));
			float4 tex2DNode165 = tex2D( _Albedo, appendResult162.xy );
			float3 lerpResult170 = lerp( tex2DNode5 , UnpackScaleNormal( tex2D( _Normals, appendResult162.xy ), _BumpScale1 ) , tex2DNode165.a);
			float3 ifLocalVar172 = 0;
			if( i.uv_texcoord.x > _MaxLeftEdgeUV )
				ifLocalVar172 = tex2DNode5;
			else if( i.uv_texcoord.x < _MaxLeftEdgeUV )
				ifLocalVar172 = lerpResult170;
			float3 ifLocalVar212 = 0;
			if( i.uv_texcoord.x > 0.5 )
				ifLocalVar212 = ifLocalVar205;
			else if( i.uv_texcoord.x < 0.5 )
				ifLocalVar212 = ifLocalVar172;
			float4 color91 = IsGammaSpace() ? float4(0.1601994,0.1601994,0.7075472,1) : float4(0.02202991,0.02202991,0.4588115,1);
			float2 uv_LinesMask34 = i.uv_texcoord;
			float4 tex2DNode34 = tex2D( _LinesMask, uv_LinesMask34 );
			float4 tex2DNode2 = tex2D( _Metallic, appendResult147.xy );
			float clampResult97 = clamp( pow( ( tex2DNode2.b * ( ( 1.0 - _LineMaskHeightThreshold ) * 10.0 ) ) , ( _LineMaskPower * 10.0 ) ) , 0.0 , 1.0 );
			float temp_output_92_0 = ( tex2DNode34.a * ( ( 1.0 - clampResult97 ) * _LineMaskPower ) );
			float4 lerpResult90 = lerp( float4( (( _ShoulderEdgeMapping )?( ifLocalVar212 ):( tex2DNode5 )) , 0.0 ) , color91 , ( temp_output_92_0 * _BumpScale3 ));
			o.Normal = lerpResult90.rgb;
			float2 uv_Detail = i.uv_texcoord * _Detail_ST.xy + _Detail_ST.zw;
			float4 tex2DNode13 = tex2D( _Detail, uv_Detail );
			float4 tex2DNode1 = tex2D( _Albedo, appendResult147.xy );
			float4 lerpResult203 = lerp( tex2DNode1 , tex2DNode198 , tex2DNode198.a);
			float4 ifLocalVar204 = 0;
			if( i.uv_texcoord.x > _MinRightEdgeUV )
				ifLocalVar204 = lerpResult203;
			else if( i.uv_texcoord.x < _MinRightEdgeUV )
				ifLocalVar204 = tex2DNode1;
			float4 lerpResult166 = lerp( tex2DNode1 , tex2DNode165 , tex2DNode165.a);
			float4 ifLocalVar171 = 0;
			if( i.uv_texcoord.x > _MaxLeftEdgeUV )
				ifLocalVar171 = tex2DNode1;
			else if( i.uv_texcoord.x < _MaxLeftEdgeUV )
				ifLocalVar171 = lerpResult166;
			float4 ifLocalVar210 = 0;
			if( i.uv_texcoord.x > 0.5 )
				ifLocalVar210 = ifLocalVar204;
			else if( i.uv_texcoord.x < 0.5 )
				ifLocalVar210 = ifLocalVar171;
			float4 temp_output_9_0 = ( (( _ShoulderEdgeMapping )?( ifLocalVar210 ):( tex2DNode1 )) * _Color );
			float4 blendOpSrc111 = ( tex2DNode13 * tex2DNode34 );
			float4 blendOpDest111 = temp_output_9_0;
			float lerpResult125 = lerp( (( tex2DNode2.b > _DetailHeightThreshold ) ? 0.0 :  1.0 ) , 1.0 , _DetailThresholdBlend);
			float4 lerpBlendMode111 = lerp(blendOpDest111,( blendOpSrc111 * blendOpDest111 ),( _DetailStrength * lerpResult125 ));
			float2 uv_Lines = i.uv_texcoord * _Lines_ST.xy + _Lines_ST.zw;
			float4 lerpResult27 = lerp( ( saturate( lerpBlendMode111 )) , ( tex2D( _Lines, uv_Lines ) * tex2DNode34 ) , temp_output_92_0);
			o.Albedo = lerpResult27.rgb;
			float4 lerpResult201 = lerp( tex2DNode2 , tex2D( _Metallic, appendResult197.xy ) , tex2DNode198.a);
			float4 ifLocalVar206 = 0;
			if( i.uv_texcoord.x > _MinRightEdgeUV )
				ifLocalVar206 = lerpResult201;
			else if( i.uv_texcoord.x < _MinRightEdgeUV )
				ifLocalVar206 = tex2DNode2;
			float4 lerpResult175 = lerp( tex2DNode2 , tex2D( _Metallic, appendResult162.xy ) , tex2DNode165.a);
			float4 ifLocalVar176 = 0;
			if( i.uv_texcoord.x > _MaxLeftEdgeUV )
				ifLocalVar176 = tex2DNode2;
			else if( i.uv_texcoord.x < _MaxLeftEdgeUV )
				ifLocalVar176 = lerpResult175;
			float4 ifLocalVar214 = 0;
			if( i.uv_texcoord.x > 0.5 )
				ifLocalVar214 = ifLocalVar206;
			else if( i.uv_texcoord.x < 0.5 )
				ifLocalVar214 = ifLocalVar176;
			float4 break186 = (( _ShoulderEdgeMapping )?( ifLocalVar214 ):( tex2DNode2 ));
			float temp_output_45_0 = ( break186.r * _MetallicPower );
			float2 uv_MetSMTyres77 = i.uv_texcoord;
			float4 tex2DNode77 = tex2D( _MetSMTyres, uv_MetSMTyres77 );
			float2 uv_MetSMTyresNoise236 = i.uv_texcoord;
			float4 tex2DNode236 = tex2D( _MetSMTyresNoise, uv_MetSMTyresNoise236 );
			float lerpResult237 = lerp( tex2DNode77.r , tex2DNode236.r , i.vertexColor.r);
			float temp_output_78_0 = ( lerpResult237 * _LanesMetallicPower );
			float ifLocalVar231 = 0;
			if( temp_output_45_0 >= temp_output_78_0 )
				ifLocalVar231 = temp_output_45_0;
			else
				ifLocalVar231 = temp_output_78_0;
			o.Metallic = ifLocalVar231;
			float temp_output_46_0 = ( break186.a * _SmoothnessPower );
			float lerpResult238 = lerp( tex2DNode77.a , tex2DNode236.a , i.vertexColor.r);
			float temp_output_83_0 = ( lerpResult238 * _LanesSmoothnessPower );
			float ifLocalVar230 = 0;
			if( temp_output_46_0 >= temp_output_83_0 )
				ifLocalVar230 = temp_output_46_0;
			else
				ifLocalVar230 = temp_output_83_0;
			float clampResult233 = clamp( ifLocalVar230 , 0.0 , 1.0 );
			o.Smoothness = clampResult233;
			o.Occlusion = ( break186.g * _OcclusionStrength1 );
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor ""
}