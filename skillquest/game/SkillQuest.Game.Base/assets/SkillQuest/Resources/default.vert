#version 330

layout ( location = 0 ) in vec3 v_vecPosition;
layout ( location = 1 ) in vec2 v_vecUv;
layout ( location = 2 ) in vec4 v_vecColor;
layout ( location = 3 ) in vec3 v_vecNormal;

uniform vec3 g_cameraPosision;
uniform vec3 g_cameraDirection;
uniform mat4 g_viewToProjection = mat4(1);
uniform mat4 g_worldToView = mat4(1);
uniform mat4 g_projectionToWorld = mat4(1);
uniform mat4 g_worldToProjection = mat4(1);
uniform mat4 g_modelToWorld = mat4(1);

out vec2 f_vecUv;

void main ( void ) {
    gl_Position = g_worldToProjection * g_modelToWorld * vec4( v_vecPosition.xyz, 1 );
    f_vecUv = v_vecUv;
}