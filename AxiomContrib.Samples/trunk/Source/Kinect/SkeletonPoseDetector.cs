#region MIT/X11 License
//Copyright (c) 2009 Axiom 3D Rendering Engine Project
//
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in
//all copies or substantial portions of the Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//THE SOFTWARE.
#endregion License

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagedNite;

namespace AxiomContrib.Samples.OpenNISample
{
	enum PoseDetectionResult
	{
		NotInPose,
		InPoseForLittleTime,
		InPoseForLongTime,
		NotEnoughConfidence,
	}

	abstract class PoseDetectorBase
	{
		#region Fields and Properties

		protected double BeginTimeOfPose;

		private double _durationOfPoseForDetection;
		protected double DurationOfPoseForDetection
		{
			get
			{
				return _durationOfPoseForDetection;
			}
			public set
			{
				_durationOfPoseForDetection = value;
				Reset();
			}
		}
		protected double DetectionPercent
		{
			public get;
			set;
		}

		#endregion  Fields and Properties

		#region Construction and Destruction

		public PoseDetectorBase( double durationForDetection = 1 )
		{
			DurationOfPoseForDetection = durationForDetection;
		}

		#endregion Construction and Destruction

		#region Methods

		public abstract PoseDetectionResult checkPose();

		public virtual void Reset()
		{
			BeginTimeOfPose = -1;
			DetectionPercent = 0;
		}

		private virtual PoseDetectionResult checkPoseDuration()
		{
			double curTime = GetCurrentTimeInSeconds();
			switch ( checkPose() )
			{
				case PoseDetectionResult.InPoseForLittleTime: //falling through
				case PoseDetectionResult.InPoseForLongTime:
				{
					if ( BeginTimeOfPose != -1 )
					{
						if ( DurationOfPoseForDetection != 0 )
						{
							DetectionPercent = ( curTime - BeginTimeOfPose ) / DurationOfPoseForDetection;
						}
						else
						{
							DetectionPercent = 1;
						}

						if ( DetectionPercent >= 1 )
						{
							DetectionPercent = 1;
							return PoseDetectionResult.InPoseForLongTime;
						}
					}
					else
					{
						BeginTimeOfPose = curTime;
					}
					return PoseDetectionResult.InPoseForLittleTime;
				}

				case PoseDetectionResult.NotEnoughConfidence:
				{
					if ( BeginTimeOfPose != -1 )
					{
						if ( ( curTime - BeginTimeOfPose ) > DurationOfPoseForDetection )
						{
							//restart waiting
							Reset();
							return PoseDetectionResult.InPoseForLittleTime;
						}
						return PoseDetectionResult.InPoseForLittleTime;
					}
					break;
				}

				case PoseDetectionResult.NotInPose:
				{

					Reset();
					break;
				}
			}
			return PoseDetectionResult.NotInPose;
		}

		public static double GetCurrentTimeInSeconds()
		{
			throw new NotImplementedException();
		}

		#endregion Methods
	}

	class EndPoseDetector: PoseDetectorBase
{
public	XnSkeletonJointTransformation m_prevLeftHand;
public		XnSkeletonJointTransformation m_prevRightHand;
	public	xn::UserGenerator m_userGenerator;
	public	XnUserID m_nUserId;

	EndPoseDetector(xn::UserGenerator ug, double duration):PoseDetectorBase(duration)
	{
		m_userGenerator = ug;
	}

	void Reset()
	{
		PoseDetectorBase::Reset();
		m_prevLeftHand.position.fConfidence = 0;
		m_prevRightHand.position.fConfidence = 0;
	}

	void SetUserId(XnUserID nUserId)
	{
		m_nUserId = nUserId;
	}

	PoseDetectionResult checkPose()
	{	
		XnSkeletonJointTransformation leftHand;
		XnSkeletonJointTransformation rightHand;
		xn::SkeletonCapability skeletonCap = m_userGenerator.GetSkeletonCap();
		
		if (!skeletonCap.IsTracking(m_nUserId))
		{
			return NOT_IN_POSE;
		}

		skeletonCap.GetSkeletonJoint(m_nUserId, XnSkeletonJoint::XN_SKEL_LEFT_HAND, leftHand);
		skeletonCap.GetSkeletonJoint(m_nUserId, XnSkeletonJoint::XN_SKEL_RIGHT_HAND, rightHand);


		bool bHaveLeftHand = leftHand.position.fConfidence  >= 0.5;
		bool bHaveRightHand = rightHand.position.fConfidence >= 0.5;
		if(!bHaveLeftHand && !bHaveRightHand )
		{
			return NOT_IN_POSE;
		}
		if(bHaveLeftHand) m_prevLeftHand  = leftHand;
		if(bHaveRightHand) m_prevRightHand = rightHand;

		//check for X (left hand is "righter" than right (more than 10 cm)
		float xDist = leftHand.position.position.X - rightHand.position.position.X ;

		if(xDist < 60 ) return NOT_IN_POSE;

		//check hands to be at same height
		float yDist = fabs(leftHand.position.position.Y - rightHand.position.position.Y);

		if(yDist > 300 ) return NOT_IN_POSE;


//		printf("in end pose!!!");
		return IN_POSE_FOR_LITTLE_TIME;
	}
}

}
