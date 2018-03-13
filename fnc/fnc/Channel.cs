using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Threading.Tasks;

namespace fnc.fnc
{
    class FadeCurve
    {
        public FadeCurve(float i_duration, float i_fadeTime)
        {
            m_fadeTime = i_fadeTime;
            m_duration = i_duration;
        }

        public float Value()
        {
            if (m_time < m_fadeTime)
                return m_time / m_fadeTime;

            float fadeStart = m_duration + m_fadeTime;
            if (m_time >= fadeStart)
                return Math.Max(0, 1.0f - (m_time - fadeStart) / m_fadeTime);

            return 1;
        }

        public void Advance(float i_timeStep)
        {
            m_time += i_timeStep;
        }

        public bool IsOver()
        {
            return m_time >= m_duration + m_fadeTime * 2;
        }

        public void Reset()
        {
            m_time = 0;
        }

        private float m_fadeTime = 3.3f;
        private float m_duration = 2.5f;
        private float m_time = 0;
    }

    class Channel
    {
        private FadeCurve m_fade;
        private Exprexssion m_express;
        private Vector3 m_value_1;
        private Vector3 m_value_2;
        private bool m_3dPlane;

        public Channel(float i_period, float i_fadeTime, bool i_3dPlane)
        {
            m_fade = new FadeCurve(i_period, i_fadeTime);
            m_3dPlane = i_3dPlane;
        }

        public void Reset()
        {
            m_express = null;
        }

        public void Update(Random i_rand, float i_timeStep)
        {
            if(m_fade.IsOver())
            {
                m_express = null;
            }
            if (m_express == null)
            {
                m_fade.Reset();
                m_value_1 = new Vector3(
                    (float)(i_rand.NextDouble()),
                    (float)(i_rand.NextDouble()),
                    (float)(i_rand.NextDouble()));
                /*m_value_2 = new Vector3(
                    (float)(i_rand.NextDouble()),
                    (float)(i_rand.NextDouble()),
                    (float)(i_rand.NextDouble())); */
                
                m_value_2 = m_value_1 / 6;
                if (m_3dPlane)
                {
                    m_express = ExperessionMaker.Make(i_rand, 3);
                    m_express = new AnimatedPlane3d(m_express, i_rand);
                }
                else
                {
                    m_express = ExperessionMaker.Make(i_rand, 5);
                }
            }

            m_fade.Advance(i_timeStep);
            m_express.Update(i_timeStep);
        }

        public void Move(float i_x, float i_y, float i_z)
        {
            //m_plane.Center += new System.Numerics.Vector3(i_x, i_y, i_z);
        }
        
        public Vector3 Value(float i_x, float i_y, float i_t)
        {
            float fade = m_fade.Value();
            
            float value = m_express.Eval(i_x, i_y, i_t);
            Vector3 color = Vector3.Lerp(m_value_1, m_value_2, value);

            return color * fade;
        }
    }
}
