#version 300 es

precision mediump float;

out vec4 FragColor;
in vec2 TexCoord;

// texture sampler
uniform sampler2D ourTexture;

void main()
{
	FragColor = texture(ourTexture, TexCoord);
}