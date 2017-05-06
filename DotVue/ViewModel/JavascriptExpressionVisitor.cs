using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web;

namespace DotVue
{
    public class JavascriptExpressionVisitor : ExpressionVisitor
    {
        private readonly StringBuilder _builder = new StringBuilder();

        public static string Resolve(Expression expr)
        {
            var v = new JavascriptExpressionVisitor();
            v.Visit(expr);
            return v.JavaScriptCode;
        }

        public string JavaScriptCode
        {
            get { return _builder.ToString(); }
        }

        public override Expression Visit(Expression node)
        {
            return base.Visit(node);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            _builder.Append(node.Name);
            return node;
        }

        protected override Expression VisitConditional(ConditionalExpression node)
        {
            this.Visit(node.Test);
            _builder.Append(" ? ");
            this.Visit(node.IfTrue);
            _builder.Append(" : ");
            this.Visit(node.IfFalse);
            return node;
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            var op = GetOperator(node.NodeType).Split('#');
            base.Visit(node.Left);
            _builder.Append(op[0]);
            base.Visit(node.Right);
            if (op.Length == 2) _builder.Append(op[1]);
            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            // for static member, Expression == null
            if(node.Expression != null)
            {
                base.VisitMember(node);
            }
            _builder.Append(ResolvedName(node.Member));
            return node;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var name = ResolvedName(node.Method).Split('#');

            // for static method, Object == null
            this.Visit(node.Object == null ? node.Arguments[0] : node.Object);

            // write first part
            _builder.Append(name[0]);

            // if has #, write parameters and finish
            if(name.Length == 2)
            {
                foreach (var arg in node.Arguments.Skip(node.Object == null ? 1 : 0))
                {
                    this.Visit(arg);
                }

                _builder.Append(name[1]);
            }

            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if(node.Value == null)
            {
                _builder.Append("null");
            }
            else if(node.Value is string || node.Value is char)
            {
                _builder.AppendFormat("'{0}'", node.Value.ToString().Replace("'", "\'"));
            }
            else if(node.Value is bool)
            {
                _builder.Append(node.Value.ToString().ToLower());
            }
            else
            {
                _builder.Append(node.Value);
            }

            return node;
        }

        protected override Expression VisitNew(NewExpression node)
        {
            if(node.Members == null || node.Members.Count == 0) throw new NotSupportedException("Expression not supported: " + node.ToString());

            _builder.Append("{ ");

            for (var i = 0; i < node.Members.Count; i++)
            {
                var member = node.Members[i];
                _builder.Append(i > 0 ? ", ": "");
                _builder.AppendFormat("'{0}': ", member.Name);
                this.Visit(node.Arguments[i]);
            }

            _builder.Append(" }");

            return node;
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            if(node.NodeType == ExpressionType.Not)
            {
                _builder.Append("!(");
                this.Visit(node.Operand);
                _builder.Append(")");
            }
            else
            {
                base.VisitUnary(node);
            }

            return node;
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            _builder.Append("function(");

            for (int i = 0; i < node.Parameters.Count; i++)
            {
                _builder.Append(i > 0 ? ", " : "");
                _builder.Append(node.Parameters[i].Name);
            }

            _builder.Append(") {");

            if (node.Body.Type != typeof(void))
            {
                _builder.Append(" return ");
            }

            base.Visit(node.Body);

            _builder.Append("; }");

            return node;
        }

        private static string GetOperator(ExpressionType nodeType)
        {
            switch (nodeType)
            {
                case ExpressionType.Add: return " + ";
                case ExpressionType.Multiply: return " * ";
                case ExpressionType.Subtract: return " - ";
                case ExpressionType.Divide: return " / ";
                case ExpressionType.Assign: return " = ";
                case ExpressionType.Equal: return " == ";
                case ExpressionType.NotEqual: return " != ";
                case ExpressionType.GreaterThan: return " > ";
                case ExpressionType.GreaterThanOrEqual: return " >= ";
                case ExpressionType.LessThan: return " < ";
                case ExpressionType.LessThanOrEqual: return " <= ";
                case ExpressionType.AndAlso: return " && ";
                case ExpressionType.OrElse: return " || ";
                case ExpressionType.ArrayIndex: return "[#]";
            }
            throw new NotSupportedException("Operator not supported: " + nodeType.ToString());
        }

        private string ResolvedName(MemberInfo member)
        {
            var isString = member.DeclaringType == typeof(string);
            var isDate = member.DeclaringType == typeof(DateTime);
            var isList = (member.DeclaringType.IsGenericType && member.DeclaringType.GetGenericTypeDefinition() == typeof(List<>)) ||
                 member.DeclaringType.IsArray ||
                 member.DeclaringType == typeof(System.Linq.Enumerable);
            var isMethod = (member as MethodInfo) != null;
            var hasParams = isMethod && ((MethodInfo)member).GetParameters().Length > 1; // extensions always have first parameter

            var name = member.Name;

            var result =
                isList && name == "Where" ? ".filter(#)" :
                isList && name == "Select" ? ".map(#)" :
                isList && name == "Length" ? ".length" :
                isList && name == "get_Item" ? "[#]" :
                isList && name == "ToArray" ? "" :
                isList && name == "ToList" ? "" :

                isList && !hasParams && name == "Count" ? ".length" :
                isList && !hasParams && name.StartsWith("First") ? "[0]" :

                isList && hasParams && name == "Count" ? ".filter(#).length" :
                isList && hasParams && name.StartsWith("First") ? ".filter(#)[0]" :

                isDate && name == "Now" ? "new Date()" :

                isString && name == "Length" ? ".length" :
                isString && name == "IndexOf" ? ".indexOf(#)" :
                isString && name == "Contains" ? ".indexOf(#) >= 0" :
                isString && name == "StartsWith" ? ".startsWith(#)" :
                isString && name == "EndsWith" ? ".endsWith(#)" :
                isString && name == "Substring" ? ".substr(#)" :
                isString && name == "Split" ? ".split(#)" :
                isString && name == "Trim" ? ".trim()" :
                isString && name == "TrimStart" ? ".trimLeft()" :
                isString && name == "TrimEnd" ? ".trimEnd()" :
                isString && name == "ToUpper" ? ".toUpperCase()" :
                isString && name == "ToLower" ? ".toLowerCase()" : null;

            if (isMethod && result == null) throw new NotSupportedException("Method not supported: " + name);
            
            return result ?? "." + name;
        }
    }
}