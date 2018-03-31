#version 330 core

in vec4 vertexColor;
in vec3 outPos;
out vec4 FragColor;
uniform vec4 ourColor;

void main()
{
    FragColor = outPos;
}