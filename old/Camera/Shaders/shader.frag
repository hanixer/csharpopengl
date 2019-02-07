#version 330 core

in vec2 TexCoord;
in vec3 ourColor;

out vec4 FragColor;

uniform sampler2D texture1;
uniform sampler2D texture2;
uniform vec3 uniformColor;

void main()
{
	vec2 newTexCoord = TexCoord;
	newTexCoord.x = 1 - newTexCoord.x;
	FragColor = mix(texture(texture1, TexCoord), texture(texture2, newTexCoord), 0.8);
	FragColor *= vec4(uniformColor, 1.0f);
}