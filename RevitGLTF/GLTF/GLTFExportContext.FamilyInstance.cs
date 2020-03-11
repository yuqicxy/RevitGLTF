﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Babylon2GLTF;
using BabylonExport.Entities;
using Utilities;

using Autodesk.Revit.DB;

namespace RevitGLTF.GLTF
{
    public partial class GLTFExportContext : IModelExportContext
    {
        //FamilyInstance开始
        public RenderNodeAction OnInstanceBegin(InstanceNode node)
        {
            var id = node.GetSymbolId();
#if DEBUG
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            var familySymbol = mRevitDocument.GetElement(id) as FamilySymbol;
            if (familySymbol == null)
                log.Error(string.Format("Instance invalid FamilySymbol:{0} => Start Failed", node.GetSymbolId()));
            else
                log.Info(String.Format("Instance FamyliySysmbol Id:{0} Name:{1} FamilyName:{2} Transform:{3} => Start",
                    node.GetSymbolId(),
                    familySymbol.Name,
                    familySymbol.FamilyName,
                    node.GetTransform().Origin.ToString()));
#endif

            //var currentMesh = mMeshStack.Peek();
            //var parentID = currentMesh.id;
            var elementid = mElementStack.Peek();

            //创建矩阵变换矩阵节点
            BabylonMesh instanceTransformNode = new BabylonMesh();
            string transformId = elementid + "_instance_" + node.GetSymbolId().ToString() + "_HashCode_" +node.GetHashCode();
            instanceTransformNode.id = transformId;
            instanceTransformNode.name = transformId;
            instanceTransformNode.parentId = elementid.ToString();
            GLTFUtil.ExportTransform(instanceTransformNode, node.GetTransform());
            mExportManager.Scene.MeshesList.Add(instanceTransformNode);

            //获取或创建实例节点
            bool hasCreated = InstanceFactory.Instance.HasCreatedIntance(id);

            //添加实例至矩阵变换节点
            List<BabylonAbstractMesh> instanceList = new List<BabylonAbstractMesh>();
            var instanceNode = InstanceFactory.Instance.GetOrCreateInstance(id);
            instanceList.Add(instanceNode);
            instanceTransformNode.instances = instanceList.ToArray();

            if (hasCreated)
            {
                //若以导出该instance，则不继续导出
                mMeshStack.Push(null);
                mMyMeshStack.Push(null);
                return RenderNodeAction.Skip;
            }
            else
            {
                mExportManager.Scene.InstancesList.Add(instanceNode);

                //添加Mesh至堆栈
                mMeshStack.Push(instanceNode);

                //创建当前MyMesh
                MyMesh mymesh = new MyMesh(instanceNode.id);
                mMyMeshStack.Push(mymesh);

                //初始化mLastMaterialID
                mLastMaterialID = null;
                return RenderNodeAction.Proceed;
            }
        }

        //FamilyInstance结束
        public void OnInstanceEnd(InstanceNode node)
        {
        #if DEBUG
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            var id = node.GetSymbolId();
            var familySymbol = mRevitDocument.GetElement(id) as FamilySymbol;
            if (familySymbol == null)
                log.Error(string.Format("Instance invalid FamilySymbol:{0} => End Failed", node.GetSymbolId()));
            else
                log.Info(String.Format("Instance FamyliySysmbol Id:{0} Name:{1} FamilyName:{2} Transform:{3} => End",
                    node.GetSymbolId(),
                    familySymbol.Name,
                    familySymbol.FamilyName,
                    node.GetTransform().Origin.ToString()));
        #endif

            //构造BabylonMesh
            BabylonMesh mesh = mMeshStack.Peek();
            var myMesh = mMyMeshStack.Peek();

            if (mesh != null && myMesh != null)
            {
                myMesh.GenerateMesh(mesh);
            }

            //析构，OnInstanceBegin创建
            mMyMeshStack.Pop();
            mMeshStack.Pop();
        }
    }
}
