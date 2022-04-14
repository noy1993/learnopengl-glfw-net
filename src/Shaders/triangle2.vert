#version 300 es

layout (location = 0) in vec3 pos;
layout (location = 1) in vec3 aColor;

out vec3 outColor;

void main()
{
    gl_Position = vec4(pos.x, pos.y, pos.z, 1.0);
    outColor = aColor;
}