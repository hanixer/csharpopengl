#version 330 core

in vec2 TexCoord;
in vec3 ourColor;

out vec4 FragColor;

uniform sampler2D texture1;
uniform sampler2D texture2;
uniform float mixRate;

void main()
{
	vec2 newTexCoord = TexCoord * 2.0;
	float p = 10.0f;
	FragColor = mix(texture(texture1, TexCoord / p + (vec2(0.5) - vec2(1.0/p/2.0))), texture(texture2, newTexCoord) * vec4(ourColor, 1.0), mixRate);
}