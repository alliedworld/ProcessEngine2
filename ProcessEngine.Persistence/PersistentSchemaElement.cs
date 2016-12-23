namespace KlaudWerk.ProcessEngine.Persistence
{
    public class PersistentSchemaElement
    {
        public int Id { get; set; }
        public virtual string SchemaBody { get; set; }
        public virtual string SchemaType { get; set; }
        public virtual string SchemaName { get; set; }
        public virtual int SerializationHint { get; set; }

        /// <summary>
        /// Copy elements
        /// </summary>
        /// <param name="sourcElement"></param>
        public void CopyFrom(PersistentSchemaElement sourcElement)
        {
            SchemaName = sourcElement.SchemaName;
            SchemaType = sourcElement.SchemaType;
            SchemaBody = sourcElement.SchemaBody;
            SerializationHint = sourcElement.SerializationHint;
        }
    }
}