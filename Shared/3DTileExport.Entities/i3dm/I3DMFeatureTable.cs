using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Tile3DExport.Entities
{
    [DataContract]
    class I3DMFeatureTable : Property
    {
        [DataMember]
        public BinaryBodyReference POSITION { get; set; }

        public bool ShouldSerializePOSITION()
        {
            if (POSITION != null)
                return true;
            else if (POSITION == null && POSITION_QUANTIZED == null)
                throw new JsonSerializationException("POSITION and POSITION_QUANTIZED can't be null at the same time");
            return false;
        }

        [DataMember]
        public BinaryBodyReference POSITION_QUANTIZED { get; set; }

        public bool ShouldSerializePOSITION_QUANTIZED()
        {
            if (POSITION_QUANTIZED != null)
            {
                if(QUANTIZED_VOLUME_OFFSET != null && QUANTIZED_VOLUME_SCALE != null)
                    return true;
                else
                    throw new JsonSerializationException("POSITION_QUANTIZED depend on QUANTIZED_VOLUME_OFFSET and QUANTIZED_VOLUME_SCALE");
            }
            else if (POSITION == null && POSITION_QUANTIZED == null)
                throw new JsonSerializationException("POSITION and POSITION_QUANTIZED can't be null at the same time");
            return false;
        }

        [DataMember]
        public BinaryBodyReference NORMAL_UP { get; set; }
        public bool ShouldSerializeNORMAL_UP()
        {
            if (NORMAL_UP != null)
            {
                if (NORMAL_RIGHT != null)
                    return true;
                else
                    throw new JsonSerializationException("NORMAL_UP depends on NORMAL_RIGHT");
            }
            else
                return false;
        }

        [DataMember]
        public BinaryBodyReference NORMAL_RIGHT { get; set; }

        public bool ShouldSerializeNORMAL_RIGHT()
        {
            if (NORMAL_RIGHT != null)
            {
                if (NORMAL_UP != null)
                    return true;
                else
                    throw new JsonSerializationException("NORMAL_RIGHT depends on NORMAL_UP");
            }
            else
                return false;
        }

        [DataMember]
        public BinaryBodyReference NORMAL_UP_OCT32P { get; set; }

        public bool ShouldSerializeNORMAL_UP_OCT32P()
        {
            if (NORMAL_UP_OCT32P != null)
            {
                if (NORMAL_RIGHT_OCT32P != null)
                    return true;
                else
                    throw new JsonSerializationException("NORMAL_UP_OCT32P depends on NORMAL_RIGHT_OCT32P");
            }
            else
                return false;
        }

        [DataMember]
        public BinaryBodyReference NORMAL_RIGHT_OCT32P { get; set; }

        public bool ShouldSerializeNORMAL_RIGHT_OCT32P()
        {
            if (NORMAL_RIGHT_OCT32P != null)
            {
                if (NORMAL_UP_OCT32P != null)
                    return true;
                else
                    throw new JsonSerializationException("NORMAL_UP_OCT32P depends on NORMAL_RIGHT_OCT32P");
            }
            else
                return false;
        }

        [DataMember(EmitDefaultValue =false)]
        public BinaryBodyReference SCALE { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public BinaryBodyReference SCALE_NON_UNIFORM { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public BinaryBodyReference BATCH_ID { get; set; }

        [DataMember]
        public GlobalPropertyScalar INSTANCES_LENGTH { get; set; }

        public bool ShouldSerializeINSTANCES_LENGTH()
        {
            if (INSTANCES_LENGTH != null)
                return true;
            else
                throw new JsonSerializationException("INSTANCES_LENGTH is required for i3dm FeatureTable");
        }

        [DataMember(EmitDefaultValue = false)]
        public GlobalPropertyCartesian3 RTC_CENTER { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GlobalPropertyCartesian3 QUANTIZED_VOLUME_OFFSET { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GlobalPropertyCartesian3 QUANTIZED_VOLUME_SCALE { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool? EAST_NORTH_UP { get; set; }
    }
}
