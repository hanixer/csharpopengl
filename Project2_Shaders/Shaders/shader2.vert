#version 330 core

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aColor;

out vec4 vertexColor;
out vec3 outPos;
uniform float horOffset;

void main()
{
    gl_Position = vec4(aPos.x + horOffset, -aPos.y, aPos.z, 1.0);
	outPos = gl_Position.xyz;
    vertexColor = vec4(aColor, 1.0);
}