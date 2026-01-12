using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GleyTrafficSystem;
using UnityEngine;

namespace Vertical
{
    /// <summary>
    /// 车辆数据点
    /// </summary>
    public class VehicleDataPoint
    {
        public float time; // 时间戳
        public Vector3 position; // 位置
        public float speed; // 速度
        public float steeringAngle; // 方向盘转角
        public float throttlePosition; // 油门踏板位置
        public float brakePosition; // 刹车踏板位置
    }

    /// <summary>
    /// 车道数据点
    /// </summary>
    public class LaneDataPoint
    {
        public float time; // 时间戳
        public Vector3 centerLinePosition; // 车道中心线位置
        public float offsetDistance; // 车道偏移距离
    }

    /// <summary>
    /// 危险事件数据
    /// </summary>
    public class RiskEventData
    {
        public float triggerTime; // 危险触发时间
        public float steeringReactionTime; // 方向盘操作反应时间
        public float brakeReactionTime; // 刹车操作反应时间
        public List<VehicleDataPoint> egoVehicleData; // 本车数据序列
        public List<VehicleDataPoint> leadVehicleData; // 前车数据序列
        public float minTTC; // 最小TTC
    }

    /// <summary>
    /// 数据统计管理类
    /// </summary>
    public class DataCenter : MonoBehaviour
    {
        private static DataCenter instance;
        public static DataCenter Instance => instance;

        #region 配置参数

        [Header("数据采集参数")] public float samplingRate = 10.0f; // 采样频率（Hz）
        private float samplingInterval; // 采样间隔（秒）

        [Header("车道参数")] public Transform laneCenterLine; // 车道中心线
        public float laneWidth = 3.75f; // 车道宽度（米）

        [Header("参考对象")] public PlayerCar playerCar; // 玩家车辆
        public G29Input g29Input; // 玩家车辆
        public GameObject leadVehicle; // 前车

        #endregion

        #region 数据存储

        private List<VehicleDataPoint> vehicleDataList = new List<VehicleDataPoint>(); // 车辆数据序列
        private List<LaneDataPoint> laneDataList = new List<LaneDataPoint>(); // 车道数据序列
        private List<RiskEventData> riskEventList = new List<RiskEventData>(); // 危险事件列表

        private float lastSamplingTime = 0; // 上次采样时间

        #endregion

        #region 当前状态

        private bool isSampling = false; // 是否正在采样
        private RiskEventData currentRiskEvent; // 当前危险事件

        #endregion

        #region 事件触发标记

        private bool steeringOperated = false; // 方向盘是否已操作
        private bool brakeOperated = false; // 刹车是否已操作

        #endregion

        #region 统计结果

        // 车道位置相关指标
        public float averageLaneOffset; // 平均车道偏移
        public float laneOffsetStdDev; // 车道偏移标准差

        // 转向操作相关指标
        public float steeringAngleStdDev; // 方向盘转角标准差

        // 速度控制指标
        public float averageSpeed; // 平均速度
        public float speedStdDev; // 速度标准差

        // 油门与刹车控制指标
        public float throttlePositionStdDev; // 油门踏板位置标准差

        // 反应时间指标
        public float steeringReactionTime; // 方向盘操作反应时间
        public float brakeReactionTime; // 刹车反应时间

        // 风险严重度指标
        public float minTTC; // 最小TTC

        #endregion

        private void Awake()
        {
            instance = this;
            samplingInterval = 1.0f / samplingRate;
        }

        private void Start()
        {
            if (playerCar == null)
            {
                playerCar = FindObjectOfType<PlayerCar>();
                g29Input = gameObject.GetComponent<G29Input>();
            }
        }

        private void Update()
        {
            if (!isSampling) return;

            // 定时采样数据
            if (Time.time - lastSamplingTime >= samplingInterval)
            {
                lastSamplingTime = Time.time;
                CollectData();
            }
        }

        /// <summary>
        /// 开始数据采集
        /// </summary>
        public void StartSampling()
        {
            isSampling = true;
            lastSamplingTime = Time.time;
            vehicleDataList.Clear();
            laneDataList.Clear();
            riskEventList.Clear();
        }

        /// <summary>
        /// 停止数据采集
        /// </summary>
        public void StopSampling()
        {
            isSampling = false;
            CalculateStatistics();
        }

        /// <summary>
        /// 触发危险事件
        /// </summary>
        public void TriggerRiskEvent()
        {
            currentRiskEvent = new RiskEventData
            {
                triggerTime = Time.time,
                egoVehicleData = new List<VehicleDataPoint>(),
                leadVehicleData = new List<VehicleDataPoint>()
            };
            riskEventList.Add(currentRiskEvent);

            steeringOperated = false;
            brakeOperated = false;
        }

        /// <summary>
        /// 采集数据
        /// </summary>
        private void CollectData()
        {
            if (playerCar == null) return;

            // 获取车辆刚体
            Rigidbody rb = playerCar.GetComponent<Rigidbody>();
            if (rb == null) return;

            // 创建车辆数据点
            VehicleDataPoint vehicleData = new VehicleDataPoint
            {
                time = Time.time,
                position = playerCar.transform.position,
                speed = rb.velocity.magnitude,
                steeringAngle = GetCurrentSteeringAngle(),
                throttlePosition = GetCurrentThrottlePosition(),
                brakePosition = GetCurrentBrakePosition()
            };

            // 添加到数据列表
            vehicleDataList.Add(vehicleData);

            // 计算车道偏移
            float offsetDistance = CalculateLaneOffset(vehicleData.position);
            LaneDataPoint laneData = new LaneDataPoint
            {
                time = Time.time,
                centerLinePosition = GetLaneCenterLinePosition(vehicleData.position.z),
                offsetDistance = offsetDistance
            };
            laneDataList.Add(laneData);

            // 如果有当前危险事件，记录数据
            if (currentRiskEvent != null)
            {
                currentRiskEvent.egoVehicleData.Add(vehicleData);

                // 获取前车数据
                if (leadVehicle != null)
                {
                    Rigidbody leadRb = leadVehicle.GetComponent<Rigidbody>();
                    if (leadRb != null)
                    {
                        VehicleDataPoint leadVehicleData = new VehicleDataPoint
                        {
                            time = Time.time,
                            position = leadVehicle.transform.position,
                            speed = leadRb.velocity.magnitude
                        };
                        currentRiskEvent.leadVehicleData.Add(leadVehicleData);
                    }
                }

                // 检查方向盘操作
                if (!steeringOperated && Mathf.Abs(vehicleData.steeringAngle) > 0.5f)
                {
                    steeringOperated = true;
                    currentRiskEvent.steeringReactionTime = vehicleData.time - currentRiskEvent.triggerTime;
                    steeringReactionTime = currentRiskEvent.steeringReactionTime;
                }

                // 检查刹车操作
                if (!brakeOperated && vehicleData.brakePosition > 0.1f)
                {
                    brakeOperated = true;
                    currentRiskEvent.brakeReactionTime = vehicleData.time - currentRiskEvent.triggerTime;
                    brakeReactionTime = currentRiskEvent.brakeReactionTime;
                }
            }
        }

        /// <summary>
        /// 计算车道偏移
        /// </summary>
        private float CalculateLaneOffset(Vector3 vehiclePosition)
        {
            // 假设车道是沿Z轴延伸的，Y轴是垂直方向
            Vector3 centerLinePos = GetLaneCenterLinePosition(vehiclePosition.z);
            float offset = vehiclePosition.x - centerLinePos.x;
            return offset;
        }

        /// <summary>
        /// 获取车道中心线位置
        /// </summary>
        private Vector3 GetLaneCenterLinePosition(float zPosition)
        {
            if (laneCenterLine != null)
            {
                // 假设车道中心线是一条直线，沿Z轴延伸
                Vector3 centerPos = laneCenterLine.position;
                return new Vector3(centerPos.x, centerPos.y, zPosition);
            }

            return new Vector3(0, 0, zPosition);
        }

        /// <summary>
        /// 获取当前方向盘转角
        /// </summary>
        private float GetCurrentSteeringAngle()
        {
            return g29Input.SteerVal;
        }

        /// <summary>
        /// 获取当前油门踏板位置
        /// </summary>
        private float GetCurrentThrottlePosition()
        {
            // 需要从输入系统获取油门踏板位置
            return g29Input.ThrottleVal;
        }

        /// <summary>
        /// 获取当前刹车踏板位置
        /// </summary>
        private float GetCurrentBrakePosition()
        {
            return g29Input.BrakeVal;
        }

        /// <summary>
        /// 计算所有统计指标
        /// </summary>
        private void CalculateStatistics()
        {
            if (vehicleDataList.Count == 0 || laneDataList.Count == 0) return;

            // 计算平均车道偏移和标准差
            averageLaneOffset = laneDataList.Average(data => Mathf.Abs(data.offsetDistance));
            float sumSquares = laneDataList.Sum(data => Mathf.Pow(data.offsetDistance - averageLaneOffset, 2));
            laneOffsetStdDev = Mathf.Sqrt(sumSquares / laneDataList.Count);

            // 计算方向盘转角标准差
            steeringAngleStdDev =
                CalculateStandardDeviation(vehicleDataList.Select(data => data.steeringAngle).ToList());

            // 计算平均速度和速度标准差
            averageSpeed = vehicleDataList.Average(data => data.speed);
            speedStdDev = CalculateStandardDeviation(vehicleDataList.Select(data => data.speed).ToList());

            // 计算油门踏板位置标准差
            throttlePositionStdDev =
                CalculateStandardDeviation(vehicleDataList.Select(data => data.throttlePosition).ToList());

            // 计算危险事件相关指标
            if (riskEventList.Count > 0)
            {
                RiskEventData latestEvent = riskEventList.Last();

                // 计算最小TTC
                latestEvent.minTTC = CalculateMinTTC(latestEvent);
                minTTC = latestEvent.minTTC;
            }

            // 输出统计结果
            var sb = new StringBuilder();
            sb.AppendLine("===== 数据统计结果 =====");
            sb.AppendLine($"平均车道偏移: {averageLaneOffset:F2} 米");
            sb.AppendLine($"车道偏移标准差: {laneOffsetStdDev:F2} 米");
            sb.AppendLine($"方向盘转角标准差: {steeringAngleStdDev:F2} 度");
            sb.AppendLine($"平均速度: {averageSpeed:F2} 米/秒");
            sb.AppendLine($"速度标准差: {speedStdDev:F2} 米/秒");
            sb.AppendLine($"油门踏板位置标准差: {throttlePositionStdDev:F2}");
            sb.AppendLine($"方向盘操作反应时间: {steeringReactionTime:F2} 秒");
            sb.AppendLine($"刹车反应时间: {brakeReactionTime:F2} 秒");
            sb.AppendLine($"最小TTC: {minTTC:F2} 秒");
            sb.AppendLine("=======================");

            //保存到本地
            var savepath = Directory.GetCurrentDirectory() + $"/DataCenter{DateTime.Now:yyyyMMddHHmmss}.txt";
            Debug.Log($"数据已保存到: {savepath}");
            FileUtility.SafeWriteAllText(savepath, sb.ToString());
        }

        /// <summary>
        /// 计算标准差
        /// </summary>
        private float CalculateStandardDeviation(List<float> values)
        {
            if (values.Count == 0) return 0;

            float mean = values.Average();
            float sumSquares = values.Sum(value => Mathf.Pow(value - mean, 2));
            float variance = sumSquares / values.Count;
            return Mathf.Sqrt(variance);
        }

        /// <summary>
        /// 计算最小TTC
        /// </summary>
        private float CalculateMinTTC(RiskEventData riskEvent)
        {
            List<float> ttcList = new List<float>();

            // 确保有足够的数据
            if (riskEvent.egoVehicleData.Count < 2 || riskEvent.leadVehicleData.Count < 2)
                return float.MaxValue;

            // 计算每个时间点的TTC
            for (int i = 0; i < riskEvent.egoVehicleData.Count; i++)
            {
                // 确保索引在范围内
                if (i >= riskEvent.leadVehicleData.Count)
                    break;

                VehicleDataPoint egoData = riskEvent.egoVehicleData[i];
                VehicleDataPoint leadData = riskEvent.leadVehicleData[i];

                // 计算相对距离
                float relativeDistance = leadData.position.z - egoData.position.z;

                // 计算相对速度
                float relativeSpeed = egoData.speed - leadData.speed;

                // 只有当相对速度大于0时，TTC才有意义
                if (relativeSpeed > 0 && relativeDistance > 0)
                {
                    float ttc = relativeDistance / relativeSpeed;
                    ttcList.Add(ttc);
                }
            }

            // 返回最小TTC，如果没有有效TTC则返回最大值
            return ttcList.Count > 0 ? ttcList.Min() : float.MaxValue;
        }

        /// <summary>
        /// 设置前车
        /// </summary>
        public void SetLeadVehicle(GameObject vehicle)
        {
            leadVehicle = vehicle;
        }
    }
}