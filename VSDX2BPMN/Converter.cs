using Aspose.Diagram;

using System.Text.RegularExpressions;

using Comindware.VSDX2BPMN.Models;

namespace Comindware.VSDX2BPMN
{
    /// <summary>
    /// Primary class for convertation from VSDX to BPMN.
    /// </summary>
    internal class Converter
    {
        private Lane _lane;
        private Models.Shape _laneShape;
        private Definition _result;
        private int _laneY;
        private List<Models.Shape> _laneShapes;
        private bool _isStarted;

        private const string СollaborationPrefix = "collaboration_";
        private const string PoolPrefix = "Pool_";
        private const string ProcessPrefix = "process_";
        private const string MainPoolName = "Main pool";
        private const string LaneSetPrefix = "laneSet_";
        private const string BPMNDiagramPrefix = "BPMNDiagram_";
        private const string BPMNPlanePrefix = "BPMNPlane_";
        private const string StartEndElementName = "Start/End";
        private const string ProcessElementName = "Process";
        private const string PageReferenceElementName = "Off-page reference";
        private const string SheetElementName = "Sheet";
        private const string DecisionElementName = "Decision";
        private const string LaneElementName = "Lane";
        private const string LanePrefix = "Lane_";
        private const string BPMNShapePrefix = "BPMNShape_";
        private const string StartPrefix = "StartEvent_";
        private const string EndPrefix = "EndEvent_";
        private const string TaskPrefix = "Task_";
        private const string AnnotationPrefix = "Annotation_";
        private const string GatewayPrefix = "Gateway_";
        private const string VisioRegex = @"<(c|p)p IX='[0-9]*'/>";
        private const int DefaultLaneY = 100;
        private const int DefaultLaneX = -100;
        private const int LaneHeight = 250;
        private const int LaneWidth = 200;
        private const int StartEndHeight = 100;
        private const int StartEndWidth = 100;
        private const int IndentLaneWidth = 200;
        private const int TaskHeight = 70;
        private const int TaskWidth = 180;
        private const int AnnotationHeight = 40;
        private const int AnnotationWidth = 160;
        private const int GatewayHeight = 70;
        private const int GatewayWidth = 70;
        private const int AverageBlockWidth = 205;
        private const int BlockIndentation = 150;

        public Converter()
        {
            _result = new Definition();
            _laneY = DefaultLaneY;
            _laneShapes = new List<Models.Shape>();
            _isStarted = false;
        }

        /// <summary>
        /// Starts convertation from VSDX to BPMN.
        /// </summary>
        /// <param name="page">VSDX page.</param>
        /// <returns>BPMN file model.</returns>
        public Definition Convert(Page page)
        {
            var shapes = page.Shapes;

            if (!shapes.IsExist(0))
            {
                return null;
            }

            var collaborationId = СollaborationPrefix + Guid.NewGuid();
            var poolId = PoolPrefix + Guid.NewGuid();
            var processRef = ProcessPrefix + Guid.NewGuid();
            _result.Collaboration = new Collaboration
            {
                Id = collaborationId,
                Participant = new Participant
                {
                    Id = poolId,
                    ProcessRef = processRef,
                    Name = MainPoolName
                }
            };

            _result.Process.Id = processRef;
            _result.Process.LaneSet.Id = LaneSetPrefix + Guid.NewGuid();

            _result.Diagram = new Models.Diagram
            {
                Id = BPMNDiagramPrefix + Guid.NewGuid(),
                Plane = new Plane
                {
                    Id = BPMNPlanePrefix + Guid.NewGuid(),
                    BpmnElement = collaborationId
                }
            };

            foreach (Aspose.Diagram.Shape shape in shapes)
            {
                if (shape.Props.Count == 0)
                {
                    continue;
                }
                else
                {
                    RenderLane(shape);
                }

                var visioText = shape?.Text?.Value?.Text;
                Regex regex = new Regex(VisioRegex);
                var text = regex.Replace(visioText, "");

                switch (shape.NameU)
                {
                    case string name when name.Contains(StartEndElementName):
                        RenderStartEnd(text);
                        break;
                    case string name when name.Contains(ProcessElementName) || name.Contains(PageReferenceElementName):
                        RenderTask(text);
                        break;
                    case string name when name.Contains(SheetElementName):
                        RenderAnnotation(text);
                        break;
                    case string name when name.Contains(DecisionElementName):
                        RenderGateway(text);
                        break;
                    default:
                        break;
                }
            }

            var max = _laneShapes.Where(x => x.BpmnElement.Contains(LaneElementName)).Max(e => e.Bounds.Width);

            foreach (var laneItem in _laneShapes)
            {
                laneItem.Bounds.Width = max;
            }

            return _result;
        }

        /// <summary>
        /// Converts lane element.
        /// </summary>
        /// <param name="shape">Element model.</param>
        private void RenderLane(Aspose.Diagram.Shape shape)
        {
            if (!shape.Props.IsExist(0))
            {
                return;
            }

            var laneName = shape.Props[0]?.Value?.Val;

            _lane = _result.Process.LaneSet.Lanes.FirstOrDefault(e => e.Name == laneName);

            if (_lane is null)
            {
                _lane = new Lane
                {
                    Id = LanePrefix + Guid.NewGuid(),
                    Name = laneName
                };

                _laneShape = new Models.Shape
                {
                    Id = BPMNShapePrefix + Guid.NewGuid(),
                    BpmnElement = _lane.Id,
                    Bounds = new Bounds
                    {
                        X = DefaultLaneX,
                        Y = _laneY,
                        Height = LaneHeight,
                        Width = LaneWidth
                    }
                };

                _laneY = _laneY + LaneHeight;

                _result.Diagram.Plane.Shapes.Add(_laneShape);
                _result.Process.LaneSet.Lanes.Add(_lane);
                _laneShapes.Add(_laneShape);
            }
            else
            {
                _laneShape = _result.Diagram.Plane.Shapes.FirstOrDefault(e => e.BpmnElement == _lane?.Id);
            }
        }

        /// <summary>
        /// Converts start and end elements.
        /// </summary>
        /// <param name="text">Element name.</param>
        private void RenderStartEnd(string text)
        {
            if (!_isStarted)
            {
                var startEventId = StartPrefix + Guid.NewGuid();

                _result.Process.StartEvent = new Element
                {
                    Id = startEventId,
                    Name = text
                };

                _result.Diagram.Plane.Shapes.Add(new Models.Shape
                {
                    Id = BPMNShapePrefix + Guid.NewGuid(),
                    BpmnElement = startEventId,
                    Bounds = new Bounds
                    {
                        Height = StartEndHeight,
                        Width = StartEndWidth,
                        X = CalculateElementX(),
                        Y = _laneShape.Bounds.Y
                    }
                });

                _lane.FlowNodeRefs.Add(startEventId);
                _isStarted = true;
            }
            else
            {
                var endEventId = EndPrefix + Guid.NewGuid();

                _result.Process.EndEvent = new Element
                {
                    Id = endEventId,
                    Name = text
                };

                _result.Diagram.Plane.Shapes.Add(new Models.Shape
                {
                    Id = BPMNShapePrefix + Guid.NewGuid(),
                    BpmnElement = endEventId,
                    Bounds = new Bounds
                    {
                        Height = StartEndHeight,
                        Width = StartEndWidth,
                        X = CalculateElementX(),
                        Y = _laneShape.Bounds.Y
                    }
                });

                _lane.FlowNodeRefs.Add(endEventId);
            }

            _laneShape.Bounds.Width += IndentLaneWidth;
        }

        /// <summary>
        /// Converts task element.
        /// </summary>
        /// <param name="text">Element name.</param>
        private void RenderTask(string text)
        {
            var taskId = TaskPrefix + Guid.NewGuid();

            _result.Process.UserTasks.Add(new Element
            {
                Id = taskId,
                Name = text
            });

            _result.Diagram.Plane.Shapes.Add(new Models.Shape
            {
                Id = BPMNShapePrefix + Guid.NewGuid(),
                BpmnElement = taskId,
                Bounds = new Bounds
                {
                    Height = TaskHeight,
                    Width = TaskWidth,
                    X = CalculateElementX(),
                    Y = _laneShape.Bounds.Y
                }
            });

            _laneShape.Bounds.Width += IndentLaneWidth;
            _lane.FlowNodeRefs.Add(taskId);
        }

        /// <summary>
        /// Converts annotation element.
        /// </summary>
        /// <param name="text">Element name.</param>
        private void RenderAnnotation(string text)
        {
            var AnnotationId = AnnotationPrefix + Guid.NewGuid();

            _result.Process.TextAnnotations.Add(new TextAnnotation
            {
                Id = AnnotationId,
                Text = text
            });

            _result.Diagram.Plane.Shapes.Add(new Models.Shape
            {
                Id = BPMNShapePrefix + Guid.NewGuid(),
                BpmnElement = AnnotationId,
                Bounds = new Bounds
                {
                    Height = AnnotationHeight,
                    Width = AnnotationWidth,
                    X = CalculateElementX(),
                    Y = _laneShape.Bounds.Y
                }
            });

            _laneShape.Bounds.Width += IndentLaneWidth;
            _lane.FlowNodeRefs.Add(AnnotationId);
        }

        /// <summary>
        /// Converts gateway element.
        /// </summary>
        /// <param name="text">Element name.</param>
        private void RenderGateway(string text)
        {
            var GatewayId = GatewayPrefix + Guid.NewGuid();

            _result.Process.InclusiveGateways.Add(new Element
            {
                Id = GatewayId,
                Name = text
            });

            _result.Diagram.Plane.Shapes.Add(new Models.Shape
            {
                Id = BPMNShapePrefix + Guid.NewGuid(),
                BpmnElement = GatewayId,
                Bounds = new Bounds
                {
                    Height = GatewayHeight,
                    Width = GatewayWidth,
                    X = CalculateElementX(),
                    Y = _laneShape.Bounds.Y
                }
            });

            _laneShape.Bounds.Width += IndentLaneWidth;
            _lane.FlowNodeRefs.Add(GatewayId);
        }

        private int CalculateElementX()
        {
            return _lane.FlowNodeRefs.Count() * AverageBlockWidth + BlockIndentation;
        }
    }
}
