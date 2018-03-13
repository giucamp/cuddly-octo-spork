using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace fnc.fnc
{
    abstract class Exprexssion
    {
        public abstract float Eval(float i_x, float i_y, float i_t);
        public abstract void Update(float i_dt);
    }

    class Sum : Exprexssion
    {
        public Sum(Exprexssion i_first, Exprexssion i_second)
        {
            m_first = i_first;
            m_second = i_second;
        }

        public override float Eval(float i_x, float i_y, float i_t)
        {
            float fv = m_first.Eval(i_x, i_y, i_t);
            float sv = m_second.Eval(i_x, i_y, i_t);
            return (fv + sv) * 0.5f;
        }

        public override void Update(float i_dt)
        {
            m_first.Update(i_dt);
            m_second.Update(i_dt);
        }

        private Exprexssion m_first, m_second;
    }

    class Product : Exprexssion
    {
        public Product(Exprexssion i_first, Exprexssion i_second)
        {
            m_first = i_first;
            m_second = i_second;
        }

        public override float Eval(float i_x, float i_y, float i_t)
        {
            float fv = m_first.Eval(i_x, i_y, i_t);
            float sv = m_second.Eval(i_x, i_y, i_t);
            return fv * sv;
        }

        public override void Update(float i_dt)
        {
            m_first.Update(i_dt);
            m_second.Update(i_dt);
        }

        private Exprexssion m_first, m_second;
    }

    class Quotient : Exprexssion
    {
        public Quotient(Exprexssion i_first, Exprexssion i_second)
        {
            m_first = i_first;
            m_second = i_second;
        }

        private float ToVal(float i_first, float i_second)
        {
            float abs_den = Math.Abs(i_second);
            float thereshold = .001f;
            if (abs_den > thereshold)
                return i_first / i_second;
            else
                return (i_first / thereshold) * i_second;
        }

        public override float Eval(float i_x, float i_y, float i_t)
        {
            return ToVal(m_first.Eval(i_x, i_y, i_t), m_second.Eval(i_x, i_y, i_t));
        }

        public override void Update(float i_dt)
        {
            m_first.Update(i_dt);
            m_second.Update(i_dt);
        }

        private Exprexssion m_first, m_second;
    }

    class Sin : Exprexssion
    {
        public Sin(Exprexssion i_first)
        {
            m_first = i_first;
        }

        public override float Eval(float i_x, float i_y, float i_t)
        {
            float arg = m_first.Eval(i_x, i_y, i_t);
            return (float)Math.Sin(arg * (20 / Math.PI));
        }

        public override void Update(float i_dt)
        {
            m_first.Update(i_dt);
        }

        private Exprexssion m_first;
    }
    
    class Atan : Exprexssion
    {
        public Atan(Exprexssion i_first)
        {
            m_first = i_first;
        }

        public override float Eval(float i_x, float i_y, float i_t)
        {
            float arg = m_first.Eval(i_x, i_y, i_t);
            return (float)(Math.Atan2(i_t + i_y, i_t + i_x) / Math.PI) * arg;
        }

        public override void Update(float i_dt)
        {
            m_first.Update(i_dt);
        }

        private Exprexssion m_first;
    }

    class TransformRotate : Exprexssion
    {
        public TransformRotate(Exprexssion i_first, Exprexssion i_second)
        {
            m_first = i_first;
            m_second = i_second;
        }

        public override float Eval(float i_x, float i_y, float i_t)
        {
            double angle = i_t * m_second.Eval(i_x, i_x, i_t) * Math.PI;
            float y = (float)Math.Sin(angle) * 10;
            float x = (float)Math.Cos(angle) * 10;
            float rx = i_x * x - i_y * y;
            float ry = i_x * y + i_y * x;
            float result = m_first.Eval(rx, ry, i_t);
            return result;
        }

        public override void Update(float i_dt)
        {
            m_first.Update(i_dt);
            m_second.Update(i_dt);
        }

        private Exprexssion m_first, m_second;
    }

    class TransformMove : Exprexssion
    {
        public TransformMove(Exprexssion i_first, Exprexssion i_second, 
            float i_deltaX, float i_deltaY,
            float i_scaleX, float i_scaleY)
        {
            m_first = i_first;
            m_second = i_second;
            m_deltaX = i_deltaX;
            m_deltaY = i_deltaY;
            m_scaleX = i_scaleX;
            m_scaleY = i_scaleY;
        }

        private static float lerp(float i_factor, float i_a, float i_b)
        {
            return i_a + (i_b - i_a) * i_factor;
        }

        public override float Eval(float i_x, float i_y, float i_t)
        {
            float factor = 10 * i_t * m_second.Eval(i_x, i_x, i_t);
            float rx = i_x * lerp(factor, 1, m_scaleX) + m_deltaX * factor;
            float ry = i_y * lerp(factor, 1, m_scaleY) * factor + m_deltaY * factor;
            float result = m_first.Eval(rx, ry, i_t);
            return result;
        }

        public override void Update(float i_dt)
        {
            m_first.Update(i_dt);
            m_second.Update(i_dt);
        }

        private Exprexssion m_first, m_second;
        private float m_deltaX, m_deltaY;
        private float m_scaleX, m_scaleY;
    }

    class TransformWave : Exprexssion
    {
        public TransformWave(Exprexssion i_first, Exprexssion i_second)
        {
            m_first = i_first;
            m_second = i_second;
        }

        public override float Eval(float i_x, float i_y, float i_t)
        {
            float freq = m_second.Eval(i_x, i_y, i_t);

            float x = (float)Math.Sin(i_t * i_x * freq * 80);
            float y = (float)Math.Sin(i_t * i_y * freq * 80);
            float result = m_first.Eval(x, y, i_t);
            return result;
        }

        public override void Update(float i_dt)
        {
            m_first.Update(i_dt);
            m_second.Update(i_dt);
        }

        private Exprexssion m_first, m_second;
    }

    class Constant : Exprexssion
    {
        public Constant(Random i_rand)
        {
            m_value = (float)i_rand.NextDouble();
        }

        public override float Eval(float i_x, float i_y, float i_t)
        {
            return m_value;
        }

        public override void Update(float i_dt)
        {
        }

        private float m_value;
    }

    class VariableX : Exprexssion
    {
        public override float Eval(float i_x, float i_y, float i_t)
        {
            return i_x;
        }

        public override void Update(float i_dt)
        {
        }
    }

    class VariableY : Exprexssion
    {
        public override float Eval(float i_x, float i_y, float i_t)
        {
            return i_y;
        }

        public override void Update(float i_dt)
        {
        }
    }

    class VariableT : Exprexssion
    {
        public override float Eval(float i_x, float i_y, float i_t)
        {
            return i_t;
        }

        public override void Update(float i_dt)
        {
        }
    }

    class Plane3d : Exprexssion
    {
        private Exprexssion m_first;
        private float m_halfFOV;
        private Vector3 m_center = new Vector3(0, -2, 0);
        private Vector3 m_normal = new Vector3(0, 1, 0);
        private Vector3 m_tangent_1 = new Vector3(1, 0, 0);
        private Vector3 m_tangent_2 = new Vector3(0, 0, 1);
        private Matrix4x4 m_rotation = Matrix4x4.Identity;
        private Vector3 m_translation = Vector3.Zero;

        public Plane3d(Exprexssion i_first, Random i_rand)
        {
            m_first = i_first;
            m_halfFOV = (float)(i_rand.NextDouble() * (300 * Math.PI / 180.0));
        }

        public float HalfFOV { get => m_halfFOV; set => m_halfFOV = value; }
        public Vector3 Center { get => m_center; set => m_center = value; }
        public Vector3 Normal { get => m_normal; set => m_normal = value; }
        public Vector3 Tangent_1 { get => m_tangent_1; set => m_tangent_1 = value; }
        public Vector3 Tangent_2 { get => m_tangent_2; set => m_tangent_2 = value; }
        public Matrix4x4 Rotation { get => m_rotation; set => m_rotation = value; }
        public Vector3 Translation { get => m_translation; set => m_translation = value; }

        public override float Eval(float i_x, float i_y, float i_t)
        {
            Vector3 ray = Utils.Spherical(i_x * m_halfFOV, i_y * m_halfFOV);

            Vector3 normal = Vector3.TransformNormal(m_normal, m_rotation);
            Vector3 center = m_center + m_translation;
            float num = Vector3.Dot(center, normal);
            float den = Vector3.Dot(ray, normal);
            if (Math.Abs(den) < 0.00001f)
                return 0;

            float dist = num / den;
            if (dist < 0)
                return 0;

            Vector3 pointInPlane = ray * dist - center;
            float u = Vector3.Dot(pointInPlane, m_tangent_1);
            float v = Vector3.Dot(pointInPlane, m_tangent_2);
            
            u = (float)Math.Sin(u);
            v = (float)Math.Sin(v);

            return m_first.Eval(u,v, i_t);
        }

        public override void Update(float i_dt)
        {
            m_first.Update(i_dt);
        }
    }

    class AnimatedPlane3d : Plane3d
    {
        Vector3 m_axis;
        Vector3 m_offset;
        float m_time;
        
        public AnimatedPlane3d(Exprexssion i_first, Random i_rand)
            : base(i_first, i_rand)
        {
            m_offset = new Vector3(
                    (float)(i_rand.NextDouble() * 2 - 1),
                    (float)(i_rand.NextDouble() * 2 - 1),
                    (float)(i_rand.NextDouble() * 2 - 1));
            m_offset *= 0.5f;

            while (m_axis.Length() <= 0.01f)
            { 
                m_axis = new Vector3(
                    (float)(i_rand.NextDouble() * 2 - 1),
                    (float)(i_rand.NextDouble() * 2 - 1),
                    (float)(i_rand.NextDouble() * 2 - 1));
            }
            m_axis.Z = m_axis.Z * 0.1f;
            m_axis = Vector3.Normalize(m_axis);
        }

        public override void Update(float i_dt)
        {
            base.Update(i_dt);

            m_time += i_dt;
            
            Rotation = Matrix4x4.CreateFromAxisAngle(m_axis, 0.2f * m_time * (float)Math.PI);
            Translation = Vector3.Transform(m_offset * (0.2f * m_time - 1.0f), Rotation);
        }
    }

    class ExperessionMaker
    {
        private delegate Exprexssion ExprFact(Random i_rand, int i_depth);

        private static ExprFact[] ExpressionMakers = {

            (Random i_rand, int i_depth)=>{ return new Sum(
                Make(i_rand, i_depth), Make(i_rand, i_depth)); },

            (Random i_rand, int i_depth)=>{ return new Product(
                Make(i_rand, i_depth), Make(i_rand, i_depth)); },

            (Random i_rand, int i_depth)=>{ return new Quotient(
                Make(i_rand, i_depth), Make(i_rand, i_depth)); },

            (Random i_rand, int i_depth)=>{ return new Atan(
                Make(i_rand, i_depth)); },
                

            (Random i_rand, int i_depth)=>{ return new Atan(
                Make(i_rand, i_depth)); },

            (Random i_rand, int i_depth)=>{ return new Sin(
                Make(i_rand, i_depth)); },

                            (Random i_rand, int i_depth)=>{ return new Sin(
                Make(i_rand, i_depth)); },

            (Random i_rand, int i_depth)=>{ return new TransformRotate(
                Make(i_rand, i_depth), Make(i_rand, i_depth)); },
                
            (Random i_rand, int i_depth)=>{ return new TransformWave(
                Make(i_rand, i_depth), Make(i_rand, i_depth)); },


            (Random i_rand, int i_depth)=>{ return new TransformMove(
                Make(i_rand, i_depth), Make(i_rand, i_depth),
                (float)i_rand.NextDouble(), (float)i_rand.NextDouble(),
                (float)i_rand.NextDouble(), (float)i_rand.NextDouble()); },
                
            (Random i_rand, int i_depth)=>{ return new AnimatedPlane3d(
                Make(i_rand, i_depth), i_rand); },
        };

        private static ExprFact[] VariableMakers = {

            (Random i_rand, int i_depth)=>{ return new Constant(i_rand); },

            (Random i_rand, int i_depth)=>{ return new VariableX(); },

            (Random i_rand, int i_depth)=>{ return new VariableY(); },

            (Random i_rand, int i_depth)=>{ return new VariableT(); }
        };

        public static Exprexssion Make(Random i_rand, int i_depth)
        {
            ExprFact factory;
            if (i_depth <= 1)
            {
                factory = VariableMakers[i_rand.Next(VariableMakers.Length)];
            }
            else
            {
                factory = ExpressionMakers[i_rand.Next(ExpressionMakers.Length)];
            }
            return factory(i_rand, i_depth - 1);
        }
    }
}
