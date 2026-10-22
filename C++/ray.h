#pragma once

#include "vectors/vec3.h"

class Ray
{
public:
    Ray() {}

    Ray(const Point3& _origin, const Vector3& _direction) 
        : Ray(_origin, _direction, 0) {}

    Ray(const Point3& _origin, const Vector3& _direction, double _time) 
        : origin(_origin), direction(_direction), time(_time) {}

    const Point3& get_origin() const { return origin; }
    const Vector3& get_direction() const { return direction; }

    double get_time() const { return time; }

    Point3 at(double t) const {return origin + t * direction; }

private:
    Point3 origin;
    Vector3 direction;
    double time;
};
