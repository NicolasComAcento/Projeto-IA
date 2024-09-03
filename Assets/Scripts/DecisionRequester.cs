// using System;
// using UnityEngine;
// using UnityEngine.Serialization;

// // O DecisionRequester é um componente que facilita a solicitação automática de decisões para uma instância de um agente (Agent) em intervalos regulares. 

// namespace Unity.MLAgents
// {
//     /// <summary>
//     /// The DecisionRequester component automatically request decisions for an
//     /// <see cref="Agent"/> instance at regular intervals.
//     /// </summary>
//     /// <remarks>
//     /// Attach a DecisionRequester component to the same [GameObject] as the
//     /// <see cref="Agent"/> component.
//     ///
//     /// The DecisionRequester component provides a convenient and flexible way to
//     /// trigger the agent decision making process. Without a DecisionRequester,
//     /// your <see cref="Agent"/> implementation must manually call its
//     /// <seealso cref="Agent.RequestDecision"/> function.
//     /// </remarks>
//     [AddComponentMenu("ML Agents/Decision Requester", (int)MenuGroup.Default)]
//     [RequireComponent(typeof(Agent))]
//     [DefaultExecutionOrder(-10)]
//     public class DecisionRequester : MonoBehaviour
//     {
//         /// <summary>
//         /// Define a frequência com que o agente solicita uma decisão. Um DecisionPeriod de 5 significa que o agente 
//         /// solicitará uma decisão a cada 5 passos da Academy.
//         /// </summary>
//         [Range(1, 20)]
//         [Tooltip("Define a frequência com que o agente solicita uma decisão. Um DecisionPeriod de 5 significa que o agente solicitará uma decisão a cada 5 passos da Academy.")]
//         public int DecisionPeriod = 5;

//         /// <summary>
//         /// Indicates when to requests a decision. By changing this value, the timing of decision
//         /// can be shifted even among agents with the same decision period. The value can be
//         /// from 0 to DecisionPeriod - 1.
//         /// </summary>
//         [Range(0, 19)]
//         [Tooltip("Indica quando solicitar uma decisão. Esse valor pode ser ajustado para deslocar o tempo de decisão entre agentes com o mesmo período de decisão.")]
//         public int DecisionStep = 0;

//         /// <summary>
//         /// Indicates whether or not the agent will take an action during the Academy steps where
//         /// it does not request a decision. Has no effect when DecisionPeriod is set to 1.
//         /// </summary>
//         [Tooltip("Indica se o agente tomará ações nos passos da Academy onde não solicita uma decisão. Não tem efeito quando DecisionPeriod é definido como 1.")]
//         [FormerlySerializedAs("RepeatAction")]
//         public bool TakeActionsBetweenDecisions = true;

//         // Referência ao componente Agent anexado ao mesmo GameObject.
//         [NonSerialized]
//         Agent m_Agent;

//         /// <summary>
//         /// Get the Agent attached to the DecisionRequester.
//         /// </summary>
//         public Agent Agent
//         {
//             get => m_Agent;
//         }

//         internal void Awake()
//         {
//             Debug.Assert(DecisionStep < DecisionPeriod, "DecisionStep must be between 0 and DecisionPeriod - 1.");
//             m_Agent = gameObject.GetComponent<Agent>();
//             Debug.Assert(m_Agent != null, "Agent component was not found on this gameObject and is required.");
//             Academy.Instance.AgentPreStep += MakeRequests;
//         }

//         void OnDestroy()
//         {
//             if (Academy.IsInitialized)
//             {
//                 Academy.Instance.AgentPreStep -= MakeRequests;
//             }
//         }

//         /// <summary>
//         /// Information about Academy step used to make decisions about whether to request a decision.
//         /// </summary>
//         public struct DecisionRequestContext
//         {
//             /// <summary>
//             /// The current step count of the Academy, equivalent to Academy.StepCount.
//             /// </summary>
//             public int AcademyStepCount;
//         }

//         /// <summary>
//         /// Method that hooks into the Academy in order inform the Agent on whether or not it should request a
//         /// decision, and whether or not it should take actions between decisions.
//         /// </summary>
//         /// <param name="academyStepCount">The current step count of the academy.</param>
//         void MakeRequests(int academyStepCount)
//         {
//             var context = new DecisionRequestContext
//             {
//                 AcademyStepCount = academyStepCount
//             };

//             if (ShouldRequestDecision(context))
//             {
//                 m_Agent?.RequestDecision();
//             }

//             if (ShouldRequestAction(context))
//             {
//                 m_Agent?.RequestAction();
//             }
//         }

//         /// <summary>
//         /// Whether Agent.RequestDecision should be called on this update step.
//         /// </summary>
//         /// <param name="context"></param>
//         /// <returns></returns>
//         protected virtual bool ShouldRequestDecision(DecisionRequestContext context)
//         {
//             return context.AcademyStepCount % DecisionPeriod == DecisionStep;
//         }

//         /// <summary>
//         /// Whether Agent.RequestAction should be called on this update step.
//         /// </summary>
//         /// <param name="context"></param>
//         /// <returns></returns>
//         protected virtual bool ShouldRequestAction(DecisionRequestContext context)
//         {
//             return TakeActionsBetweenDecisions;
//         }
//     }
// }
