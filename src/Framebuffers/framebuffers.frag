#version 300 es

precision mediump float;

out vec4 FragColor;
in vec2 TexCoords;
uniform sampler2D texture1;
uniform bool selected;
void main()
{    
    if(selected){
        FragColor = vec4(1,1,0,0.5);    
    }
    else{
        FragColor =1.0- texture(texture1, TexCoords);
    }
    
    //FragColor = texture(texture1, TexCoords);
    
}