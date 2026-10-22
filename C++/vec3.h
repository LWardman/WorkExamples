#pragma once 

class Vector3
{
    public:
        double vec[3];

        Vector3() : vec{0, 0, 0} {}

        Vector3(double x, double y, double z) : vec{x, y, z} {}

        double x() const { return vec[0]; }
        double y() const { return vec[1]; }
        double z() const { return vec[2]; }

        Vector3 operator-() const { return Vector3(-x(), -y(), -z()); }
        double operator[](int i) const { return vec[i]; }
        double& operator[](int i) { return vec[i]; }

        Vector3& operator+=(const Vector3& other)
        {
            vec[0] += other.x();
            vec[1] += other.y();
            vec[2] += other.z();
            return *this;
        }

        Vector3& operator*=(double multiplier)
        {
            vec[0] *= multiplier;
            vec[1] *= multiplier;
            vec[2] *= multiplier;
            return *this;
        }

        Vector3& operator/=(double denominator)
        {
            return *this *= 1 / denominator;
        }

        double magnitude() const
        {
            return std::sqrt(magnitude_squared());
        }

        double magnitude_squared() const
        {
            return x()*x() + y()*y() + z()*z();
        }

        bool near_zero() const
        {
            double tolerance = 1e-8;

            return (std::fabs(vec[0]) < tolerance) && (std::fabs(vec[1]) < tolerance) && (std::fabs(vec[2]) < tolerance);
        }

        static Vector3 random()
        {
            return Vector3(random_double(), random_double(), random_double());
        }

        static Vector3 random(double min, double max)
        {
            return Vector3(random_double(min, max), random_double(min, max), random_double(min, max));
        }
};

using Point3 = Vector3;

inline std::ostream& operator<<(std::ostream& out, const Vector3& v)
{
    return out << v.x() << ' ' << v.y() << ' ' << v.z();
}

inline Vector3 operator+(const Vector3 u, const Vector3 v)
{
    return Vector3(u.x() + v.x(), u.y() + v.y(), u.z() + v.z());
}

inline Vector3 operator-(const Vector3 u, const Vector3 v)
{
    return Vector3(u.x() - v.x(), u.y() - v.y(), u.z() - v.z());
}

inline Vector3 operator*(const Vector3 u, const Vector3 v)
{
    return Vector3(u.x() * v.x(), u.y() * v.y(), u.z() * v.z());
}

inline Vector3 operator*(double t, const Vector3& v)
{
    return Vector3(t*v.x(), t*v.y(), t*v.z());
}

inline Vector3 operator*(const Vector3& v, double t)
{
    return t * v;
}

inline Vector3 operator/(const Vector3& v, double t)
{
    return (1/t) * v;
}

inline double dot(const Vector3& u, const Vector3& v)
{
    return u.x() * v.x() + u.y() * v.y() + u.z() * v.z();
}

inline Vector3 cross(const Vector3& u, const Vector3& v)
{
    double x = u.y() * v.z() - u.z() * v.y();
    double y = u.z() * v.x() - u.x() * v.z();
    double z = u.x() * v.y() - u.y() * v.x();

    return Vector3(x, y, z);
}

inline Vector3 unit_vector(const Vector3& v)
{
    return v / v.magnitude();
}

inline Vector3 random_unit_vector()
{
    while (true)
    {
        Vector3 point = Vector3::random(-1, 1);
        double mag_sqr = point.magnitude_squared();

        if (1e-160 && mag_sqr <= 1) 
            return point / point.magnitude();
            
    }
}

inline Vector3 random_on_hemisphere(const Vector3& normal)
{
    Vector3 on_unit_sphere = random_unit_vector();
    if (dot(on_unit_sphere, normal) > 0.0f) // Is in the same hemisphere as the normal
        return on_unit_sphere;
    return -on_unit_sphere;
}

inline Vector3 reflect(const Vector3& vector, const Vector3& normal)
{
    return vector - 2 * dot(vector, normal) * normal;
}

inline Vector3 refract(const Vector3& uv, const Vector3& normal, double etai_over_etat)
{
    double cos_theta = std::fmin(dot(-uv, normal), 1.0f);
    Vector3 r_out_perp = etai_over_etat * (uv + cos_theta * normal);
    Vector3 r_out_parallel = -std::sqrt(std::fabs(1.0f - r_out_perp.magnitude_squared())) * normal;
    return r_out_perp + r_out_parallel;
}

inline Vector3 random_in_unit_disk()
{
    while (true)
    {
        Vector3 point = Vector3(random_double(-1, 1), random_double(-1, 1), 0);
        if (point.magnitude_squared() < 1)
            return point;
    }
}
