using System.Linq.Expressions;

namespace Gamepedia.Lol.Api.Extensions
{
	internal static class ExpressionTypeExtensions
	{
		/// <summary>
		/// Converts an <see cref="ExpressionType"/> object to its <see cref="string"/> representation.
		/// </summary>
		/// <remarks>
		/// This method only converts <see cref="ExpressionType"/> objects that are also common SQL operators.
		/// </remarks>
		/// <param name="expressionType">The <see cref="ExpressionType"/>.</param>
		/// <returns>A <see cref="string"/> representation of the operator.</returns>
		/// <exception cref="System.InvalidOperationException">When <paramref name="expressionType"/> is not a common SQL operator.</exception>
		internal static string ToSqlOperator(this ExpressionType expressionType)
		{
			return expressionType switch
			{
				ExpressionType.Equal => "=",
				ExpressionType.GreaterThan => ">",
				ExpressionType.LessThan => "<",
				ExpressionType.GreaterThanOrEqual => ">=",
				ExpressionType.LessThanOrEqual => "<=",
				ExpressionType.NotEqual => "<>",
				ExpressionType.AndAlso => "AND",
				ExpressionType.OrElse => "OR",
				ExpressionType.Not => "NOT",
				_ => throw new System.InvalidOperationException(),
			};
		}
	}
}