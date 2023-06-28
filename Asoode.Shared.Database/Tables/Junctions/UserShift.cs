using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables.Junctions;

public class UserShift : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid GroupId { get; set; }
    public Guid ShiftId { get; set; }
    public int Hours { get; set; }
    public double Salary { get; set; }
}