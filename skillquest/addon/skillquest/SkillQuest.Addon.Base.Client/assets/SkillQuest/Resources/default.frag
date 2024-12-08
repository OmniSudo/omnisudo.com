#version 330

in vec2 f_vecUv;

out vec4 r_vecUv;

uniform sampler2D textureSampler;

void main ( void ) {
    r_vecUv = texture( textureSampler, f_vecUv );
    // fragmentColor = vec4( 1, 1, 0, 1 );
    // fragmentColor = voxelColor;
}