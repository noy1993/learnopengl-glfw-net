#version 300 es

precision mediump float;

out vec4 FragColor;

in vec3 Normal;
in vec3 FragPos;
in vec2 TexCoords;

struct DirLight {
    vec3 direction;
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

struct Material {
    sampler2D diffuse;
    vec3 specular;
    float shininess;
};

uniform vec3 viewPos;
uniform Material material;
uniform DirLight light;
vec3 CalDirLight(DirLight light,vec3 normal,vec3 viewDir);

void main(){ 
    vec3 viewDir = normalize(viewPos - FragPos);
    FragColor = vec4(CalDirLight(light,Normal,viewDir), 1.0);
}

vec3 CalDirLight(DirLight light, vec3 normal, vec3 viewDir)
{
    vec3 lightDir = normalize(-light.direction);
    float diff = max(dot(normal, lightDir), 0.0);
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    vec3 ambient  = light.ambient  *        vec3(texture(material.diffuse, TexCoords));
    vec3 diffuse  = light.diffuse  * diff * vec3(texture(material.diffuse, TexCoords));
    vec3 specular = light.specular * spec * material.specular;

    return (ambient + diffuse + specular);
}