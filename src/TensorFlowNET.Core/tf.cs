﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using TF_DataType = Tensorflow.DataType;
using attr_value_pb2 = Tensorflow;
using Tensorflow.Eager;

namespace Tensorflow
{
    public static class tf
    {
        public static DataType float32 = DataType.DtFloat;

        public static Context context = new Context();

        public static Graph g = new Graph(c_api.TF_NewGraph());

        public delegate void Deallocator(IntPtr data, IntPtr size, IntPtr deallocatorData);

        public static unsafe Tensor add(Tensor a, Tensor b)
        {
            return gen_math_ops.add(a, b);
        }

        public static unsafe Tensor placeholder(DataType dtype, TensorShape shape = null)
        {
            return gen_array_ops.placeholder(dtype, shape);
        }

        public static unsafe Tensor constant(object value)
        {
            var g = ops.get_default_graph();
            var tensor_value = new attr_value_pb2.AttrValue();
            var tensor_pb = tensor_util.make_tensor_proto(value);
            tensor_value.Tensor = tensor_pb;
            var dtype_value = new attr_value_pb2.AttrValue
            {
                Type = tensor_value.Tensor.Dtype,
            };

            var attrs = new Dictionary<string, AttrValue>();
            attrs["dtype"] = dtype_value;
            attrs["value"] = tensor_value;
            var const_tensor = g.create_op("Const", null, new TF_DataType[] { dtype_value.Type }, attrs: attrs).outputs[0];

            return const_tensor;
        }

        public static void enable_eager_execution()
        {
            context.default_execution_mode = Context.EAGER_MODE;
        }

        public static Deallocator FreeTensorDataDelegate = FreeTensorData;

        [MonoPInvokeCallback(typeof(Deallocator))]
        internal static void FreeTensorData(IntPtr data, IntPtr len, IntPtr closure)
        {
            Marshal.FreeHGlobal(data);
        }

        public static string VERSION => Marshal.PtrToStringAnsi(c_api.TF_Version());

        public static Graph get_default_graph()
        {
            return ops.get_default_graph();
        }

        public static Graph Graph()
        {
            return g;
        }

        public static Session Session()
        {
            return new Session();
        }
    }
}
