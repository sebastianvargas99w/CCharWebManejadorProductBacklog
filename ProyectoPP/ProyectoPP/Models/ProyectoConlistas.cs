using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoPP.Models
{
    public class ProyectoConlistas
    {

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProyectoConlistas()
        {
            this.historiasDeUsuario = new HashSet<historiasDeUsuario>();
            this.sprint = new HashSet<sprint>();
            this.persona = new HashSet<persona>();
        }

        public string id { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public System.DateTime fechaInicio { get; set; }
        public Nullable<System.DateTime> fechaFinal { get; set; }
        public string lider { get; set; }
        public string estado { get; set; }
        public string[] listaAgregar { get; set; }
        public string[] listaQuitar { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<historiasDeUsuario> historiasDeUsuario { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<sprint> sprint { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<persona> persona { get; set; }
        public virtual persona persona1 { get; set; }

    }
}
