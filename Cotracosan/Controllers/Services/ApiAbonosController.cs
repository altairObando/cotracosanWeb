using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cotracosan.Models.Cotracosan;
using System.Threading.Tasks;
using System.Data.Entity;


namespace Cotracosan.Controllers.Services
{
    public class ApiAbonosController : Controller
    {
        private Context db = new Context();
        // AgregarAbono
        public JsonResult AddAbono(DateTime FechaDeAbono, decimal MontoDeAbono, int CreditoId)
        {
            using (var transact = db.Database.BeginTransaction())
            {
                try
                {
                    var abono = new Abonos
                    {
                        CodigoAbono = "ABO",
                        CreditoId = CreditoId,
                        Creditos = null,
                        Estado = true,
                        FechaDeAbono = FechaDeAbono,
                        Id = 0,
                        MontoDeAbono = MontoDeAbono
                    };
                    // Guardar el abono.
                    db.Abonos.Add(abono);
                    bool guardado = db.SaveChanges() > 0;
                    bool codigoAbono = false;
                    if(guardado)
                    {
                        // Actualizamos el codigo del abono con el nuevo Id
                        db.Entry(abono).State = EntityState.Detached; // separamos del que tiene seguimiento y buscamos el nuevo para evitar errores.
                        abono = db.Abonos.Find(abono.Id);
                        abono.CodigoAbono = "ABO-" + abono.Id;
                        db.Entry(abono).State = EntityState.Modified;
                        codigoAbono = db.SaveChanges() > 0;
                    }
                    if (guardado && codigoAbono)
                        transact.Commit();

                    return Json(new {
                        guardado = guardado && codigoAbono,
                        mensaje = guardado && codigoAbono ? "Se ha registrado el abono." :
                                  !guardado && !codigoAbono ? "Error al guardar el abono" :
                                  guardado && !codigoAbono ? "Se ha guardado el abono, pero no se logro generar el codigo" :
                                  "error general."
                    }, JsonRequestBehavior.AllowGet);
                } 
                catch (Exception e)
                {
                    transact.Rollback();
                    return Json(new {  }, JsonRequestBehavior.AllowGet);
                }
            }
        }
        // Eliminar Abono
        public JsonResult DeleteAbono(int abonoId)
        {
            // Buscar el abono
            var abono = db.Abonos.Find(abonoId);
            if(abono == null)
                return Json(new { eliminado = false, mensaje = "No existe ningun abono con el id: " + abonoId }, JsonRequestBehavior.AllowGet);
            abono.Estado = false;
            db.Entry(abono).State = EntityState.Modified;

            bool guardado = db.SaveChanges() > 0;
            return Json(new {
                eliminado = guardado,
                mensaje = guardado ? "Se ha eliminado el abono" : "No se ha logrado eliminar el abono, error del servidor"
            }, JsonRequestBehavior.AllowGet);
        }
    }
}