using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace ProyectoApiContable.Entities
{
    [Table("partidas_cuentas", Schema = "contable")]
    public class PartidasCuentas
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }
        public int PartidaContableId { get; set; }

        [ForeignKey(nameof(PartidaContableId))]
        public virtual PartidasContables PartidaContable { get; set; }

        public int CatalogoCuentaId { get; set; }
        [ForeignKey(nameof(CatalogoCuentaId))]
        public virtual CatalogoCuentas CatalogoCuentas { get; set; }

    }
}