#version 330 core

in vec2 TexCoord;
in vec3 ourColor;

out vec4 FragColor;

uniform sampler2D texture1;
uniform sampler2D texture2;

void main()
{
	vec2 newTexCoord = TexCoord * 2.0;
	FragColor = mix(texture(texture1, newTexCoord), texture(texture2, newTexCoord) * vec4(ourColor, 1.0), 0.4);
}