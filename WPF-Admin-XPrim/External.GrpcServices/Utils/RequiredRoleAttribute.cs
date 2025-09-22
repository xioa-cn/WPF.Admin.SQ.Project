namespace External.GrpcServices.Utils
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RequiredRoleAttribute : Attribute
    {
        public string[] Roles { get; }
        public RequiredRoleAttribute(params string[] roles) => Roles = roles;
    }
}