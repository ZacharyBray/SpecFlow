using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow.Bindings;
using System.Reflection;

namespace TechTalk.SpecFlow.CustomBindings
{
    public enum MethodType
    {
        DoubleTicked, // E.g., this.``When $trader buys $amount of $product``(...)
        Standard      // E.g., this.When_trader_buys_amount_of_product(...)
    };

    public class PublicMethodsAreStepBindingsAttribute : CustomBindingAttribute
    {
        private readonly MethodType methodType;

        public PublicMethodsAreStepBindingsAttribute(MethodType methodType)
        {
            this.methodType = methodType;
        }

        public override IEnumerable<StepBinding> CreateBindings(MethodInfo methodInfo)
        {
            var pattern = methodInfo.Name;

            if (methodType == MethodType.Standard)
            {
                pattern = pattern.Replace('_', ' ');
            }

            foreach (var parameter in methodInfo.GetParameters())
            {
                if (methodType == MethodType.DoubleTicked)
                {
                    pattern = pattern.Replace("$" + parameter.Name, "(.*)");
                }
                else
                {
                    pattern = pattern.Replace(parameter.Name, "(.*)");
                }
            }

            if (pattern.StartsWith("Given"))
            {
                pattern = pattern.Substring(5).Trim();
                yield return new StepBinding(BindingType.Given, pattern, methodInfo);
            }
            else if (pattern.StartsWith("When"))
            {
                pattern = pattern.Substring(4).Trim();
                yield return new StepBinding(BindingType.When, pattern, methodInfo);
            }
            else if (pattern.StartsWith("Then"))
            {
                pattern = pattern.Substring(4).Trim();
                yield return new StepBinding(BindingType.Then, pattern, methodInfo);
            }
        }
    }
}
