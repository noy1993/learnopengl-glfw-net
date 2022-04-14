#version 300 es

precision mediump float;

out vec4 FragColor;
in vec3 outColor;

void main()
{
    FragColor = vec4(outColor,1.0);
} 