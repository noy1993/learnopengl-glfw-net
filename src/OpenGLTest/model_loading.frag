#version 300 es

precision mediump float;

out vec4 FragColor;
 
struct DirLight {
    vec3 direction;
	
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

in vec2 TexCoords;
in vec3 Normal;
in vec3 FragPos;

struct Material {
    sampler2D diffuse;
    sampler2D specular;
    float shininess;
}; 

uniform vec3 viewPos;

uniform DirLight dirLight;
uniform Material material1;

vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir);
void main()
{    
	// FragColor = texture(material1.diffuse, TexCoords);

	vec3 viewDir = normalize(viewPos - FragPos);
    FragColor = vec4(CalcDirLight(dirLight,Normal,viewDir),1.0);
}

// calculates the color when using a directional light.
vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir)
{
    vec3 lightDir = normalize(-light.direction);
    // diffuse shading
    float diff = max(dot(normal, lightDir), 0.0);
    // specular shading
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material1.shininess);
    // combine results
    vec3 ambient = light.ambient * vec3(texture(material1.diffuse, TexCoords));
    vec3 diffuse = light.diffuse * diff * vec3(texture(material1.diffuse, TexCoords));
    if(ambient.x == 0.0 && ambient.y == 0.0 && ambient.z == 0.0) ambient = vec3(0.7,0.7,0.7);
    vec3 specular = light.specular * spec * vec3(texture(material1.specular, TexCoords));
    return (ambient + diffuse + specular);
}