namespace Neon.HomeControl.Api.Core.Data.UserInteraction
{
	public class UserInteractionField
	{
		public string FieldName { get; set; }

		public UserInteractionFieldTypeEnum FieldType { get; set; }

		public object FieldValue { get; set; }

		public bool IsRequired { get; set; }

		public string Description { get; set; }

		public UserInteractionField Build()
		{
			return this;
		}

		public UserInteractionField SetFieldName(string value)
		{
			FieldName = value;
			return this;
		}

		public UserInteractionField SetFieldValue(object value)
		{
			FieldValue = value;
			return this;
		}

		public UserInteractionField SetFieldType(UserInteractionFieldTypeEnum value)
		{
			FieldType = value;
			return this;
		}

		public UserInteractionField SetIsRequired(bool value)
		{
			IsRequired = value;

			return this;
		}

		public UserInteractionField SetDescription(string value)
		{
			Description = value;

			return this;
		}
	}
}