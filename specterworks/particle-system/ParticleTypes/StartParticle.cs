using particle_system;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace specterworks.ParticleTypes
{
    public class StartParticle : EmitingParticle
    {
        public ObservableCollection<StartingParticle> StartingParticles { get; set; } = new ObservableCollection<StartingParticle>();

        public override double? EmitPeriod { get; set; } = 0.1;
        private int EmitIndex = 0;
        protected override void Emit(Action<Particle> emitter)
        {
            foreach (var p in StartingParticles)
            {
                if (p.Take <= 0)
                    continue;
                if (p.Skip > 0)
                {
                    p.Skip--;
                    continue;
                }
                if (p.Every != 0)
                    p.Skip = p.Every - 1;
                p.Take--;

                emitter(p.TemplateParticle);
            }
            EmitIndex++;
        }
    }

    public class StartingParticle
    {
        [Editor(typeof(ParticleTypeCustomEditor), typeof(ParticleTypeCustomEditor))]
        [ExpandableObject]
        public Particle TemplateParticle { get; set; }
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 1;
        public int Every { get; set; } = 1;
    }
}
