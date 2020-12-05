using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

using Gamepedia.Lol.Client.Extensions;
using Gamepedia.Lol.Client.Interfaces;

namespace Gamepedia.Lol.Client.Options
{
	public class SqlOptions<T> where T : ICargoTables
	{
		private readonly StringBuilder where;
		private readonly StringBuilder orderBy;
		private readonly List<string>? fields;

		public SqlOptions(ref List<string>? fields, ref StringBuilder where, ref StringBuilder orderBy)
		{
			this.where = where;
			this.fields = fields;
			this.orderBy = orderBy;
		}

		/// <summary>
		/// Creates a <c>where</c> query that corresponds to a <c>SQL WHERE</c> clause.
		/// </summary>
		/// <param name="expression">The expression that defines the query.</param>
		public SqlOptions<T> Where(Expression<Func<T, bool>> expression)
		{
			// Boolean operations are always binary expressions
			var rootExpression = expression.Body as BinaryExpression;

			// The left side of the expression must be a property of T,
			// therefore it should always be a member expression
			if (rootExpression?.Left is MemberExpression leftMemberExpression)
			{
				// Adds the current left property name as a filed if not already added
				if (fields?.Contains(leftMemberExpression.Member.Name) is false)
				{
					fields.Add(leftMemberExpression.Member.Name);
				}

				where.Clear();
				where.Append($"A.{leftMemberExpression.Member.Name} " +
					$"{rootExpression.NodeType.ToSqlOperator()} ");

				// Right side of the expression is a constant value, i.e a number or a string
				if (rootExpression.Right is ConstantExpression rightConstantExpression)
				{
					where.Append($"'{rightConstantExpression.Value}'");
					return this;
				}

				// Right side of the expression is an object instantiation
				else if (rootExpression.Right is NewExpression rightNewExpression)
				{
					var arguments = new object[rightNewExpression.Arguments.Count];

					// Try casting parameters to constant values
					foreach (var argument in rightNewExpression.Arguments)
					{
						if (argument is ConstantExpression constant)
						{
							arguments[rightNewExpression.Arguments.IndexOf(argument)] = constant.Value;
						}

						else
						{
							throw new NotImplementedException();
						}
					}

					// Creates the object
					var obj = rightNewExpression.Constructor.Invoke(arguments);

					if (obj is DateTime dateTime)
					{
						// Format DateTime as SQL DateTime
						where.Append($"'{dateTime:s}'");
						return this;
					}

					else
					{
						where.Append($"'{obj}'");
						return this;
					}
				}

				throw new NotImplementedException($"Expression {expression} could not be converted to a SQL Query.");
			}

			throw new InvalidOperationException($"The left side of the expression '{expression}' must be a property of {typeof(T).Name}.");
		}
		public SqlOptions<T> And(Expression<Func<T, bool>> expression)
		{
			throw new NotImplementedException();
		}

		public SqlOptions<T> Or(Expression<Func<T, bool>> expression)
		{
			throw new NotImplementedException();
		}

		public void Where(string query)
		{
			where.Clear();
			where.Append(query);
		}

		public void OrderBy(Expression<Func<T, object>> expression, Order orderBy)
		{
			throw new NotImplementedException();
		}

		public void OrderBy(string query)
		{
			orderBy.Append(query);
		}
	}

	public enum Order
	{
		Ascending,
		Descending
	}
}