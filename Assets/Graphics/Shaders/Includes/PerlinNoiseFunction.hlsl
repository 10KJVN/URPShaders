// Perlin Noise Function

float fade(float t) {
    return t * t * t * (t * (t * 6 - 15) + 10);
}

float lerp(float a, float b, float t) {
    return a + t * (b - a);
}

float grad(int hash, float x, float y) {
    int h = hash & 3;
    float u = h < 2 ? x : y;
    float v = h < 2 ? y : x;
    return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
}

int hash(int x, int y) {
    int h = (x + y * 57) & 255;
    return h;
}

float perlinNoise(float2 pos) {
    int x0 = floor(pos.x);
    int x1 = x0 + 1;
    int y0 = floor(pos.y);
    int y1 = y0 + 1;

    float2 f = float2(pos.x - x0, pos.y - y0);
    float2 fadeF = float2(fade(f.x), fade(f.y));

    int h00 = hash(x0, y0);
    int h01 = hash(x0, y1);
    int h10 = hash(x1, y0);
    int h11 = hash(x1, y1);

    float n00 = grad(h00, f.x, f.y);
    float n01 = grad(h01, f.x, f.y - 1.0);
    float n10 = grad(h10, f.x - 1.0, f.y);
    float n11 = grad(h11, f.x - 1.0, f.y - 1.0);

    float nX0 = lerp(n00, n10, fadeF.x);
    float nX1 = lerp(n01, n11, fadeF.x);
    return lerp(nX0, nX1, fadeF.y);
}
