using VecompSoftware.DocSuiteWeb.Entity.Parameters;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Parameters
{
    public class ParameterEnvMapper : BaseEntityMapper<ParameterEnv, ParameterEnv>, IParameterEnvMapper
    {
        public ParameterEnvMapper()
        {

        }

        public override ParameterEnv Map(ParameterEnv entity, ParameterEnv entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Name = entity.Name;
            entityTransformed.Value = entity.Value;
            #endregion

            return entityTransformed;
        }

    }
}
